using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xnova;
using Xnova.API.RequestModel;
using Xnova.Models;

namespace Xnova.API.Controllers
{
    [ApiController]
    [Route("api/owner/reports")]
    public class OwnerReportController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly XnovaContext _context;

        public OwnerReportController(UnitOfWork unitOfWork, XnovaContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        // GET: api/owner/reports/revenue
        [HttpGet("revenue")]
        [Authorize]
        public async Task<IActionResult> GetRevenueReport(
            [FromQuery] string period = "month",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? fieldIds = null, // comma-separated ids
            [FromQuery] string groupBy = "day")
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == null)
            {
                return Unauthorized(new { message = "Missing or invalid user token" });
            }

            var (from, to) = ResolveDateRange(period, startDate, endDate);
            var (prevFrom, prevTo) = PreviousRange(from, to);

            // All field ids owned by this owner
            var ownerFieldIdsQuery = _context.Fields
                .Include(f => f.Venue)
                .Where(f => f.Venue != null && f.Venue.UserId == ownerId);

            if (!string.IsNullOrWhiteSpace(fieldIds))
            {
                var ids = fieldIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => int.TryParse(s, out var v) ? v : (int?)null)
                    .Where(v => v.HasValue)
                    .Select(v => v!.Value)
                    .ToHashSet();
                ownerFieldIdsQuery = ownerFieldIdsQuery.Where(f => ids.Contains(f.Id));
            }

            var ownerFields = await ownerFieldIdsQuery
                .Select(f => new { f.Id, f.Name, VenueAddress = f.Venue!.Address })
                .ToListAsync();
            var ownerFieldIdsList = ownerFields.Select(f => f.Id).ToList();

            // Payments considered as revenue (Status == 1)
            var payments = await _context.Payments
                .Include(p => p.Booking)
                .Where(p => p.Status == 1
                            && p.Date != null
                            && p.Booking != null
                            && p.Booking.FieldId != null
                            && ownerFieldIdsList.Contains(p.Booking.FieldId.Value)
                            && p.Date!.Value.Date >= from.Date
                            && p.Date!.Value.Date <= to.Date)
                .ToListAsync();

            var prevPayments = await _context.Payments
                .Include(p => p.Booking)
                .Where(p => p.Status == 1
                            && p.Date != null
                            && p.Booking != null
                            && p.Booking.FieldId != null
                            && ownerFieldIdsList.Contains(p.Booking.FieldId.Value)
                            && p.Date!.Value.Date >= prevFrom.Date
                            && p.Date!.Value.Date <= prevTo.Date)
                .ToListAsync();

            var totalRevenue = payments.Sum(p => (decimal)(p.Amount ?? 0));
            var previousRevenue = prevPayments.Sum(p => (decimal)(p.Amount ?? 0));
            var change = previousRevenue == 0 ? (totalRevenue > 0 ? 100m : 0m) : Math.Round(((totalRevenue - previousRevenue) / previousRevenue) * 100m, 2);
            var trend = change > 0 ? "up" : (change < 0 ? "down" : "stable");

            var daysCount = Math.Max(1, (to.Date - from.Date).Days + 1);
            var weeksCount = Math.Max(1, (int)Math.Ceiling(daysCount / 7m));
            var averageDaily = Math.Round(totalRevenue / daysCount, 2);
            var averageWeekly = Math.Round(totalRevenue / weeksCount, 2);

            // Time series
            var timeSeries = payments
                .GroupBy(p => GroupKey(p.Date!.Value, groupBy))
                .Select(g => new
                {
                    key = g.Key,
                    revenue = g.Sum(x => (decimal)(x.Amount ?? 0)),
                    bookings = g.Select(x => x.BookingId).Distinct().Count(),
                })
                .OrderBy(x => x.key)
                .ToList();

            var peak = timeSeries.OrderByDescending(x => x.revenue).FirstOrDefault();
            var lowest = timeSeries.OrderBy(x => x.revenue).FirstOrDefault();

            var timeSeriesData = timeSeries.Select(x => new RevenueTimePoint
            {
                Date = x.key,
                Revenue = Math.Round(x.revenue, 2),
                Bookings = x.bookings,
                AverageBookingValue = x.bookings == 0 ? 0 : Math.Round(x.revenue / x.bookings, 2)
            }).ToList();

            // Field breakdown
            var fieldRevenue = payments
                .Where(p => p.Booking!.FieldId.HasValue)
                .GroupBy(p => p.Booking!.FieldId!.Value)
                .Select(g => new
                {
                    FieldId = g.Key,
                    Revenue = g.Sum(x => (decimal)(x.Amount ?? 0)),
                    Bookings = g.Select(x => x.BookingId).Distinct().Count()
                })
                .ToList();

            var totalForPercentage = fieldRevenue.Sum(x => x.Revenue);

            // Previous field revenue for growth rate
            var prevFieldRevenue = prevPayments
                .Where(p => p.Booking!.FieldId.HasValue)
                .GroupBy(p => p.Booking!.FieldId!.Value)
                .ToDictionary(g => g.Key, g => g.Sum(x => (decimal)(x.Amount ?? 0)));

            var fieldBreakdown = fieldRevenue
                .Select(x =>
                {
                    var info = ownerFields.FirstOrDefault(f => f.Id == x.FieldId);
                    var prev = prevFieldRevenue.ContainsKey(x.FieldId) ? prevFieldRevenue[x.FieldId] : 0m;
                    var growth = prev == 0 ? (x.Revenue > 0 ? 100m : 0m) : Math.Round(((x.Revenue - prev) / prev) * 100m, 2);
                    return new FieldBreakdown
                    {
                        FieldId = x.FieldId.ToString(),
                        FieldName = info?.Name ?? $"Field {x.FieldId}",
                        Location = info?.VenueAddress ?? string.Empty,
                        Revenue = Math.Round(x.Revenue, 2),
                        Percentage = totalForPercentage == 0 ? 0 : Math.Round((x.Revenue / totalForPercentage) * 100m, 2),
                        Bookings = x.Bookings,
                        AverageBookingValue = x.Bookings == 0 ? 0 : Math.Round(x.Revenue / x.Bookings, 2),
                        GrowthRate = growth
                    };
                })
                .OrderByDescending(f => f.Revenue)
                .ToList();

            // Hourly breakdown
            var hourlyBreakdown = payments
                .Where(p => p.Date != null)
                .GroupBy(p => p.Date!.Value.Hour)
                .Select(g => new HourlyBreakdown
                {
                    Hour = g.Key,
                    Revenue = g.Sum(x => (decimal)(x.Amount ?? 0)),
                    Bookings = g.Select(x => x.BookingId).Distinct().Count()
                })
                .ToList();
            var hourlyTotal = hourlyBreakdown.Sum(h => h.Revenue);
            hourlyBreakdown.ForEach(h => h.Percentage = hourlyTotal == 0 ? 0 : Math.Round((h.Revenue / hourlyTotal) * 100m, 2));

            // Weekday breakdown
            var weekdayBreakdown = payments
                .Where(p => p.Date != null)
                .GroupBy(p => (int)p.Date!.Value.DayOfWeek)
                .Select(g => new WeekdayBreakdown
                {
                    DayOfWeek = g.Key,
                    DayName = CultureInfo.InvariantCulture.DateTimeFormat.GetDayName((DayOfWeek)g.Key),
                    Revenue = g.Sum(x => (decimal)(x.Amount ?? 0)),
                    Bookings = g.Select(x => x.BookingId).Distinct().Count()
                })
                .OrderBy(x => x.DayOfWeek)
                .ToList();
            var weekdayTotal = weekdayBreakdown.Sum(w => w.Revenue);
            weekdayBreakdown.ForEach(w => w.Percentage = weekdayTotal == 0 ? 0 : Math.Round((w.Revenue / weekdayTotal) * 100m, 2));

            // Payment methods
            var methodBreakdown = payments
                .GroupBy(p => p.Method ?? "unknown")
                .Select(g => new PaymentMethodBreakdown
                {
                    Method = g.Key,
                    Revenue = g.Sum(x => (decimal)(x.Amount ?? 0)),
                    TransactionCount = g.Count()
                })
                .ToList();
            var methodTotal = methodBreakdown.Sum(m => m.Revenue);
            methodBreakdown.ForEach(m => m.Percentage = methodTotal == 0 ? 0 : Math.Round((m.Revenue / methodTotal) * 100m, 2));

            // Top customers
            var topCustomers = await _context.Payments
                .Include(p => p.Booking)!.ThenInclude(b => b!.User)
                .Where(p => p.Status == 1
                            && p.Date != null
                            && p.Booking != null
                            && p.Booking.FieldId != null
                            && ownerFieldIdsList.Contains(p.Booking.FieldId.Value)
                            && p.Date!.Value.Date >= from.Date
                            && p.Date!.Value.Date <= to.Date)
                .GroupBy(p => new { p.Booking!.UserId, p.Booking!.User!.Name })
                .Select(g => new TopCustomer
                {
                    CustomerId = (g.Key.UserId ?? 0).ToString(),
                    CustomerName = g.Key.Name ?? "Unknown",
                    TotalSpent = g.Sum(x => (decimal)(x.Amount ?? 0)),
                    BookingCount = g.Select(x => x.BookingId).Distinct().Count(),
                })
                .OrderByDescending(x => x.TotalSpent)
                .Take(10)
                .ToListAsync();
            topCustomers.ForEach(c => c.AverageBookingValue = c.BookingCount == 0 ? 0 : Math.Round(c.TotalSpent / c.BookingCount, 2));

            var result = new RevenueReportResponse
            {
                ReportId = $"rev_{DateTime.UtcNow:yyyy_MM_dd_HHmmss}",
                GeneratedAt = DateTime.UtcNow,
                Period = period,
                StartDate = from.Date,
                EndDate = to.Date,
                Summary = new RevenueSummary
                {
                    TotalRevenue = Math.Round(totalRevenue, 2),
                    PreviousPeriodRevenue = Math.Round(previousRevenue, 2),
                    Change = change,
                    Trend = trend,
                    AverageDaily = Math.Round(averageDaily, 2),
                    AverageWeekly = Math.Round(averageWeekly, 2),
                    PeakDay = peak == null ? null : new DayRevenue { Date = peak.key, Revenue = Math.Round(peak.revenue, 2) },
                    LowestDay = lowest == null ? null : new DayRevenue { Date = lowest.key, Revenue = Math.Round(lowest.revenue, 2) }
                },
                TimeSeriesData = timeSeriesData,
                FieldBreakdown = fieldBreakdown,
                HourlyBreakdown = hourlyBreakdown,
                WeekdayBreakdown = weekdayBreakdown,
                PaymentMethodBreakdown = methodBreakdown,
                TopCustomers = topCustomers
            };

            return Ok(result);
        }

        // GET: api/owner/reports/bookings
        [HttpGet("bookings")]
        [Authorize]
        public async Task<IActionResult> GetBookingReport(
            [FromQuery] string period = "month",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? fieldIds = null)
        {
            var ownerId = GetCurrentUserId();
            if (ownerId == null)
            {
                return Unauthorized(new { message = "Missing or invalid user token" });
            }

            var (from, to) = ResolveDateRange(period, startDate, endDate);

            var ownerFieldIdsQuery = _context.Fields
                .Include(f => f.Venue)
                .Where(f => f.Venue != null && f.Venue.UserId == ownerId);

            if (!string.IsNullOrWhiteSpace(fieldIds))
            {
                var ids = fieldIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => int.TryParse(s, out var v) ? v : (int?)null)
                    .Where(v => v.HasValue)
                    .Select(v => v!.Value)
                    .ToHashSet();
                ownerFieldIdsQuery = ownerFieldIdsQuery.Where(f => ids.Contains(f.Id));
            }
            var ownerFieldIdsList = await ownerFieldIdsQuery.Select(f => f.Id).ToListAsync();

            var bookings = await _context.Bookings
                .Where(b => b.FieldId != null
                            && ownerFieldIdsList.Contains(b.FieldId!.Value)
                            && b.Date != null
                            && b.Date!.Value.ToDateTime(TimeOnly.MinValue).Date >= from.Date
                            && b.Date!.Value.ToDateTime(TimeOnly.MinValue).Date <= to.Date)
                .ToListAsync();

            // Map bookings to payments for revenue
            var bookingIds = bookings.Select(b => b.Id).ToList();
            var bookingPayments = await _context.Payments
                .Where(p => p.Status == 1 && p.BookingId != null && bookingIds.Contains(p.BookingId.Value))
                .ToListAsync();

            var totalBookings = bookings.Count;
            var confirmedBookings = bookingPayments.Select(p => p.BookingId).Distinct().Count();
            var cancelledBookings = bookings.Count(b => b.Status == 2); // Assumption: 2 = cancelled
            var completedBookings = confirmedBookings; // Simplified assumption
            var pendingBookings = totalBookings - confirmedBookings - cancelledBookings;
            if (pendingBookings < 0) pendingBookings = 0;

            var totalRevenue = bookingPayments.Sum(p => (decimal)(p.Amount ?? 0));
            var avgBookingValue = totalBookings == 0 ? 0 : Math.Round(totalRevenue / totalBookings, 2);

            var timeSeries = bookings
                .GroupBy(b => b.Date!.Value.ToDateTime(TimeOnly.MinValue).ToString("yyyy-MM-dd"))
                .Select(g => new BookingTimePoint
                {
                    Date = g.Key,
                    Bookings = g.Count(),
                    Confirmed = g.Select(x => x.Id).Intersect(bookingPayments.Select(p => p.BookingId ?? 0)).Count(),
                    Cancelled = g.Count(x => x.Status == 2),
                    Revenue = bookingPayments.Where(p => g.Select(x => x.Id).Contains(p.BookingId ?? -1)).Sum(p => (decimal)(p.Amount ?? 0))
                })
                .OrderBy(x => x.Date)
                .ToList();

            var response = new BookingReportResponse
            {
                ReportId = $"book_{DateTime.UtcNow:yyyy_MM_dd_HHmmss}",
                GeneratedAt = DateTime.UtcNow,
                Period = period,
                StartDate = from.Date,
                EndDate = to.Date,
                Summary = new BookingSummary
                {
                    TotalBookings = totalBookings,
                    ConfirmedBookings = confirmedBookings,
                    CancelledBookings = cancelledBookings,
                    CompletedBookings = completedBookings,
                    PendingBookings = pendingBookings,
                    CancellationRate = totalBookings == 0 ? 0 : Math.Round((decimal)cancelledBookings / totalBookings * 100m, 2),
                    ConfirmationRate = totalBookings == 0 ? 0 : Math.Round((decimal)confirmedBookings / totalBookings * 100m, 2),
                    AverageBookingValue = avgBookingValue,
                    TotalRevenue = Math.Round(totalRevenue, 2)
                },
                TimeSeriesData = timeSeries
            };

            return Ok(response);
        }

        // Helpers
        private static (DateTime from, DateTime to) ResolveDateRange(string period, DateTime? start, DateTime? end)
        {
            if (start.HasValue && end.HasValue)
            {
                return (start.Value.Date, end.Value.Date);
            }

            var today = DateTime.UtcNow.Date;
            return period?.ToLower() switch
            {
                "week" => (today.AddDays(-6), today),
                "month" => (new DateTime(today.Year, today.Month, 1), new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month))),
                "quarter" =>
                    (new DateTime(today.Year, ((today.Month - 1) / 3) * 3 + 1, 1),
                     new DateTime(today.Year, ((today.Month - 1) / 3) * 3 + 1, 1).AddMonths(3).AddDays(-1)),
                "year" => (new DateTime(today.Year, 1, 1), new DateTime(today.Year, 12, 31)),
                _ => (today.AddDays(-29), today) // default 30 days
            };
        }

        private static (DateTime from, DateTime to) PreviousRange(DateTime from, DateTime to)
        {
            var length = (to - from).Days + 1;
            var prevTo = from.AddDays(-1);
            var prevFrom = prevTo.AddDays(-(length - 1));
            return (prevFrom.Date, prevTo.Date);
        }

        private static string GroupKey(DateTime dateTime, string groupBy)
        {
            return groupBy?.ToLower() switch
            {
                "week" => ISOWeek.GetYear(dateTime) + "-W" + ISOWeek.GetWeekOfYear(dateTime).ToString("00"),
                "month" => dateTime.ToString("yyyy-MM"),
                _ => dateTime.ToString("yyyy-MM-dd")
            };
        }

        private int? GetCurrentUserId()
        {
            var sub = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(sub, out var id) ? id : (int?)null;
        }

        // DTOs moved to Xnova.API.RequestModel (OwnerReportDtos.cs)
    }
}