using System;
using System.Collections.Generic;

namespace Xnova.API.RequestModel
{
    // Revenue Report DTOs
    public class RevenueReportResponse
    {
        public string ReportId { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public string Period { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public RevenueSummary Summary { get; set; } = new();
        public List<RevenueTimePoint> TimeSeriesData { get; set; } = new();
        public List<FieldBreakdown> FieldBreakdown { get; set; } = new();
        public List<HourlyBreakdown> HourlyBreakdown { get; set; } = new();
        public List<WeekdayBreakdown> WeekdayBreakdown { get; set; } = new();
        public List<PaymentMethodBreakdown> PaymentMethodBreakdown { get; set; } = new();
        public List<TopCustomer> TopCustomers { get; set; } = new();
    }

    public class RevenueSummary
    {
        public decimal TotalRevenue { get; set; }
        public decimal PreviousPeriodRevenue { get; set; }
        public decimal Change { get; set; }
        public string Trend { get; set; } = "stable";
        public decimal AverageDaily { get; set; }
        public decimal AverageWeekly { get; set; }
        public DayRevenue? PeakDay { get; set; }
        public DayRevenue? LowestDay { get; set; }
    }

    public class DayRevenue
    {
        public string Date { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }

    public class RevenueTimePoint
    {
        public string Date { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int Bookings { get; set; }
        public decimal AverageBookingValue { get; set; }
    }

    public class FieldBreakdown
    {
        public string FieldId { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public decimal Percentage { get; set; }
        public int Bookings { get; set; }
        public decimal AverageBookingValue { get; set; }
        public decimal GrowthRate { get; set; }
    }

    public class HourlyBreakdown
    {
        public int Hour { get; set; }
        public decimal Revenue { get; set; }
        public int Bookings { get; set; }
        public decimal Percentage { get; set; }
    }

    public class WeekdayBreakdown
    {
        public int DayOfWeek { get; set; }
        public string DayName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int Bookings { get; set; }
        public decimal Percentage { get; set; }
    }

    public class PaymentMethodBreakdown
    {
        public string Method { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public decimal Percentage { get; set; }
        public int TransactionCount { get; set; }
    }

    public class TopCustomer
    {
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalSpent { get; set; }
        public int BookingCount { get; set; }
        public decimal AverageBookingValue { get; set; }
    }

    // Booking Report DTOs
    public class BookingReportResponse
    {
        public string ReportId { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public string Period { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public BookingSummary Summary { get; set; } = new();
        public List<BookingTimePoint> TimeSeriesData { get; set; } = new();
    }

    public class BookingSummary
    {
        public int TotalBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int PendingBookings { get; set; }
        public decimal CancellationRate { get; set; }
        public decimal ConfirmationRate { get; set; }
        public decimal AverageBookingValue { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class BookingTimePoint
    {
        public string Date { get; set; } = string.Empty;
        public int Bookings { get; set; }
        public int Confirmed { get; set; }
        public int Cancelled { get; set; }
        public decimal Revenue { get; set; }
    }
}