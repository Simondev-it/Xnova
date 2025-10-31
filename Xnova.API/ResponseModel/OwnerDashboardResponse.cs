using System;
using System.Collections.Generic;

namespace Xnova.API.ResponseModel
{
    /// <summary>
    /// Response model cho Dashboard Overview
    /// </summary>
    public class DashboardStatsResponse
    {
        public RevenueData Revenue { get; set; } = new RevenueData();
        public UserData Users { get; set; } = new UserData();
        public BookingData Bookings { get; set; } = new BookingData();
        public FieldData Fields { get; set; } = new FieldData();
    }

    public class RevenueData
    {
        public decimal Total { get; set; }
        public decimal Today { get; set; }
        public decimal Week { get; set; }
        public decimal Month { get; set; }
        public List<decimal> Monthly { get; set; } = new List<decimal>();
        public double Change { get; set; }
        public string Trend { get; set; } = "stable";
    }

    public class UserData
    {
        public int Total { get; set; }
        public int NewUsers { get; set; }
        public int Daily { get; set; }
        public int Weekly { get; set; }
        public double Change { get; set; }
        public string Trend { get; set; } = "stable";
    }

    public class BookingData
    {
        public int Total { get; set; }
        public List<int> Daily { get; set; } = new List<int>();
        public List<BookingActivityItem> Activity { get; set; } = new List<BookingActivityItem>();
        public double Change { get; set; }
        public string Trend { get; set; } = "stable";
    }

    public class BookingActivityItem
    {
        public string Date { get; set; } = string.Empty;
        public int Bookings { get; set; }
        public decimal Revenue { get; set; }
        public double AverageBookingValue { get; set; }
    }

    public class FieldData
    {
        public int Total { get; set; }
        public int Active { get; set; }
        public int Hidden { get; set; }
        public int Maintenance { get; set; }
        public List<TopFieldItem> TopFields { get; set; } = new List<TopFieldItem>();
    }

    public class TopFieldItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Bookings { get; set; }
        public decimal Revenue { get; set; }
        public decimal PricePerHour { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsVisible { get; set; }
        public double? Rating { get; set; }
        public int Reviews { get; set; }
    }

    /// <summary>
    /// Response model cho Revenue theo period
    /// </summary>
    public class RevenueByPeriodResponse
    {
        public string Period { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
        public decimal PreviousPeriodRevenue { get; set; }
        public double Change { get; set; }
        public string Trend { get; set; } = "stable";
        public List<DailyRevenueItem> TimeSeriesData { get; set; } = new List<DailyRevenueItem>();
        public List<FieldRevenueItem> FieldBreakdown { get; set; } = new List<FieldRevenueItem>();
    }

    public class DailyRevenueItem
    {
        public string Date { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int Bookings { get; set; }
    }

    public class FieldRevenueItem
    {
        public string FieldId { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public double Percentage { get; set; }
        public int Bookings { get; set; }
    }

    /// <summary>
    /// Response model cho Bookings theo period
    /// </summary>
    public class BookingsByPeriodResponse
    {
        public string Period { get; set; } = string.Empty;
        public int TotalBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int PendingBookings { get; set; }
        public double CancellationRate { get; set; }
        public List<BookingActivityItem> Activity { get; set; } = new List<BookingActivityItem>();
        public List<FieldBookingItem> FieldBreakdown { get; set; } = new List<FieldBookingItem>();
    }

    public class FieldBookingItem
    {
        public string FieldId { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public int Bookings { get; set; }
        public double Percentage { get; set; }
        public double OccupancyRate { get; set; }
    }

    /// <summary>
    /// Response model cho Users theo period
    /// </summary>
    public class UsersByPeriodResponse
    {
        public string Period { get; set; } = string.Empty;
        public int TotalUsers { get; set; }
        public int NewUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int ReturningUsers { get; set; }
        public double RetentionRate { get; set; }
        public List<UserGrowthItem> UserGrowth { get; set; } = new List<UserGrowthItem>();
    }

    public class UserGrowthItem
    {
        public string Date { get; set; } = string.Empty;
        public int NewUsers { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
    }
}
