using System;
using System.Collections.Generic;

namespace Xnova.API.ResponseModel
{
    /// <summary>
    /// Base response structure cho tất cả API responses
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public string Timestamp { get; set; } = DateTime.UtcNow.ToString("o");
    }

    /// <summary>
    /// Error response structure
    /// </summary>
    public class ApiErrorResponse
    {
        public bool Success { get; set; } = false;
        public ErrorDetail Error { get; set; } = new ErrorDetail();
        public string Timestamp { get; set; } = DateTime.UtcNow.ToString("o");
    }

    public class ErrorDetail
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public object? Details { get; set; }
    }

    /// <summary>
    /// Response model cho Revenue Report
    /// </summary>
    public class RevenueReportResponse
    {
        public string ReportId { get; set; } = string.Empty;
        public string GeneratedAt { get; set; } = DateTime.UtcNow.ToString("o");
        public string Period { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public RevenueSummary Summary { get; set; } = new RevenueSummary();
        public List<TimeSeriesDataItem> TimeSeriesData { get; set; } = new List<TimeSeriesDataItem>();
        public List<FieldBreakdownItem> FieldBreakdown { get; set; } = new List<FieldBreakdownItem>();
        public List<HourlyBreakdownItem> HourlyBreakdown { get; set; } = new List<HourlyBreakdownItem>();
        public List<WeekdayBreakdownItem> WeekdayBreakdown { get; set; } = new List<WeekdayBreakdownItem>();
        public List<PaymentMethodItem> PaymentMethodBreakdown { get; set; } = new List<PaymentMethodItem>();
        public List<TopCustomerItem> TopCustomers { get; set; } = new List<TopCustomerItem>();
    }

    public class RevenueSummary
    {
        public decimal TotalRevenue { get; set; }
        public decimal PreviousPeriodRevenue { get; set; }
        public double Change { get; set; }
        public string Trend { get; set; } = "stable";
        public double AverageDaily { get; set; }
        public double AverageWeekly { get; set; }
        public PeakDayInfo? PeakDay { get; set; }
        public PeakDayInfo? LowestDay { get; set; }
    }

    public class PeakDayInfo
    {
        public string Date { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }

    public class TimeSeriesDataItem
    {
        public string Date { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int Bookings { get; set; }
        public double AverageBookingValue { get; set; }
    }

    public class FieldBreakdownItem
    {
        public string FieldId { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public double Percentage { get; set; }
        public int Bookings { get; set; }
        public double AverageBookingValue { get; set; }
        public double GrowthRate { get; set; }
    }

    public class HourlyBreakdownItem
    {
        public int Hour { get; set; }
        public decimal Revenue { get; set; }
        public int Bookings { get; set; }
        public double Percentage { get; set; }
    }

    public class WeekdayBreakdownItem
    {
        public int DayOfWeek { get; set; }
        public string DayName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int Bookings { get; set; }
        public double Percentage { get; set; }
    }

    public class PaymentMethodItem
    {
        public string Method { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public double Percentage { get; set; }
        public int TransactionCount { get; set; }
    }

    public class TopCustomerItem
    {
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalSpent { get; set; }
        public int BookingCount { get; set; }
        public double AverageBookingValue { get; set; }
    }

    /// <summary>
    /// Response model cho Booking Report
    /// </summary>
    public class BookingReportResponse
    {
        public string ReportId { get; set; } = string.Empty;
        public string GeneratedAt { get; set; } = DateTime.UtcNow.ToString("o");
        public string Period { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public BookingSummary Summary { get; set; } = new BookingSummary();
        public List<TimeSeriesDataItem> TimeSeriesData { get; set; } = new List<TimeSeriesDataItem>();
        public List<FieldBookingDetailItem> FieldBookings { get; set; } = new List<FieldBookingDetailItem>();
        public List<HourlyDistributionItem> HourlyDistribution { get; set; } = new List<HourlyDistributionItem>();
        public List<WeekdayDistributionItem> WeekdayDistribution { get; set; } = new List<WeekdayDistributionItem>();
        public DurationAnalysis DurationAnalysis { get; set; } = new DurationAnalysis();
        public AdvanceBookingAnalysis AdvanceBookingAnalysis { get; set; } = new AdvanceBookingAnalysis();
        public CancellationAnalysis CancellationAnalysis { get; set; } = new CancellationAnalysis();
        public List<PeakPeriodItem> PeakPeriods { get; set; } = new List<PeakPeriodItem>();
    }

    public class BookingSummary
    {
        public int TotalBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int PendingBookings { get; set; }
        public double CancellationRate { get; set; }
        public double ConfirmationRate { get; set; }
        public double AverageBookingValue { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class FieldBookingDetailItem
    {
        public string FieldId { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public int TotalBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public double OccupancyRate { get; set; }
        public decimal Revenue { get; set; }
        public double AverageBookingDuration { get; set; }
        public List<int> PeakHours { get; set; } = new List<int>();
    }

    public class HourlyDistributionItem
    {
        public int Hour { get; set; }
        public int Bookings { get; set; }
        public double Percentage { get; set; }
        public double AverageRevenue { get; set; }
    }

    public class WeekdayDistributionItem
    {
        public int DayOfWeek { get; set; }
        public string DayName { get; set; } = string.Empty;
        public int Bookings { get; set; }
        public double Percentage { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DurationAnalysis
    {
        public double Average { get; set; }
        public int Median { get; set; }
        public int Mode { get; set; }
        public List<DurationGroupItem> Distribution { get; set; } = new List<DurationGroupItem>();
    }

    public class DurationGroupItem
    {
        public int Duration { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class AdvanceBookingAnalysis
    {
        public AdvanceBookingItem SameDay { get; set; } = new AdvanceBookingItem();
        public AdvanceBookingItem OneDayAdvance { get; set; } = new AdvanceBookingItem();
        public AdvanceBookingItem ThreeDaysAdvance { get; set; } = new AdvanceBookingItem();
        public AdvanceBookingItem OneWeekAdvance { get; set; } = new AdvanceBookingItem();
        public AdvanceBookingItem MoreThanWeek { get; set; } = new AdvanceBookingItem();
    }

    public class AdvanceBookingItem
    {
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class CancellationAnalysis
    {
        public int TotalCancelled { get; set; }
        public List<CancellationReasonItem> Reasons { get; set; } = new List<CancellationReasonItem>();
        public CancellationTiming TimeBeforeCancellation { get; set; } = new CancellationTiming();
    }

    public class CancellationReasonItem
    {
        public string Reason { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class CancellationTiming
    {
        public int LessThan1Hour { get; set; }
        public int LessThan24Hours { get; set; }
        public int LessThan3Days { get; set; }
        public int MoreThan3Days { get; set; }
    }

    public class PeakPeriodItem
    {
        public string Period { get; set; } = string.Empty;
        public int Bookings { get; set; }
        public decimal Revenue { get; set; }
        public double OccupancyRate { get; set; }
    }

    /// <summary>
    /// Response model cho User Report
    /// </summary>
    public class UserReportResponse
    {
        public string ReportId { get; set; } = string.Empty;
        public string GeneratedAt { get; set; } = DateTime.UtcNow.ToString("o");
        public string Period { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public UserSummary Summary { get; set; } = new UserSummary();
        public List<UserGrowthDataItem> UserGrowth { get; set; } = new List<UserGrowthDataItem>();
        public List<UserSegmentItem> UserSegments { get; set; } = new List<UserSegmentItem>();
        public ActivityMetrics ActivityMetrics { get; set; } = new ActivityMetrics();
        public EngagementMetrics EngagementMetrics { get; set; } = new EngagementMetrics();
        public List<TopUserItem> TopUsers { get; set; } = new List<TopUserItem>();
    }

    public class UserSummary
    {
        public int TotalUsers { get; set; }
        public int NewUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int ReturningUsers { get; set; }
        public double ChurnRate { get; set; }
        public double RetentionRate { get; set; }
        public double AverageLifetimeValue { get; set; }
        public double AverageBookingsPerUser { get; set; }
        public double AverageRevenuePerUser { get; set; }
    }

    public class UserGrowthDataItem
    {
        public string Date { get; set; } = string.Empty;
        public int NewUsers { get; set; }
        public int ActiveUsers { get; set; }
    }

    public class UserSegmentItem
    {
        public string Segment { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
        public double AverageRevenue { get; set; }
        public double AverageBookings { get; set; }
    }

    public class ActivityMetrics
    {
        public double AverageBookingsPerUser { get; set; }
        public double AverageRevenuePerUser { get; set; }
        public double AverageSessionDuration { get; set; }
        public double AverageTimeOnSite { get; set; }
        public double BounceRate { get; set; }
    }

    public class EngagementMetrics
    {
        public int DailyActiveUsers { get; set; }
        public int WeeklyActiveUsers { get; set; }
        public int MonthlyActiveUsers { get; set; }
        public double DauWauRatio { get; set; }
        public double DauMauRatio { get; set; }
    }

    public class TopUserItem
    {
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalSpent { get; set; }
        public string? LastBooking { get; set; }
        public string? MemberSince { get; set; }
    }

    /// <summary>
    /// Response model cho Performance Report
    /// </summary>
    public class PerformanceReportResponse
    {
        public string ReportId { get; set; } = string.Empty;
        public string GeneratedAt { get; set; } = DateTime.UtcNow.ToString("o");
        public string Period { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public OverallPerformance OverallPerformance { get; set; } = new OverallPerformance();
        public List<FieldPerformanceItem> FieldPerformance { get; set; } = new List<FieldPerformanceItem>();
        public KPIs Kpis { get; set; } = new KPIs();
        public BenchmarkComparison BenchmarkComparison { get; set; } = new BenchmarkComparison();
        public List<RecommendationItem> Recommendations { get; set; } = new List<RecommendationItem>();
        public List<GrowthOpportunityItem> GrowthOpportunities { get; set; } = new List<GrowthOpportunityItem>();
    }

    public class OverallPerformance
    {
        public int Score { get; set; }
        public string Rating { get; set; } = string.Empty;
        public int PreviousScore { get; set; }
        public int Change { get; set; }
        public List<string> Strengths { get; set; } = new List<string>();
        public List<string> Weaknesses { get; set; } = new List<string>();
    }

    public class FieldPerformanceItem
    {
        public string FieldId { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int Bookings { get; set; }
        public decimal Revenue { get; set; }
        public double OccupancyRate { get; set; }
        public double UtilizationRate { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public int PerformanceScore { get; set; }
        public int RevenueScore { get; set; }
        public int OccupancyScore { get; set; }
        public int SatisfactionScore { get; set; }
        public string BookingTrend { get; set; } = string.Empty;
        public string RevenueTrend { get; set; } = string.Empty;
        public string RatingTrend { get; set; } = string.Empty;
        public List<PeakHourItem> PeakHours { get; set; } = new List<PeakHourItem>();
        public List<IssueItem> Issues { get; set; } = new List<IssueItem>();
    }

    public class PeakHourItem
    {
        public int Hour { get; set; }
        public int Bookings { get; set; }
        public int Revenue { get; set; }
    }

    public class IssueItem
    {
        public string Type { get; set; } = string.Empty;
        public int Count { get; set; }
        public string Severity { get; set; } = string.Empty;
    }

    public class KPIs
    {
        public double AverageOccupancyRate { get; set; }
        public double AverageBookingValue { get; set; }
        public double CustomerSatisfaction { get; set; }
        public double RepeatCustomerRate { get; set; }
        public double CancellationRate { get; set; }
        public double ResponseTime { get; set; }
        public double MaintenanceDowntime { get; set; }
    }

    public class BenchmarkComparison
    {
        public BenchmarkItem Industry { get; set; } = new BenchmarkItem();
        public BenchmarkItem TopPerformers { get; set; } = new BenchmarkItem();
        public BenchmarkItem CustomerSatisfaction { get; set; } = new BenchmarkItem();
    }

    public class BenchmarkItem
    {
        public double AverageOccupancy { get; set; }
        public double YourOccupancy { get; set; }
        public double Difference { get; set; }
        public double AverageRevenue { get; set; }
        public double YourRevenue { get; set; }
        public double IndustryAverage { get; set; }
        public double YourAverage { get; set; }
    }

    public class RecommendationItem
    {
        public string Priority { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ExpectedImpact { get; set; } = string.Empty;
        public List<string> ActionItems { get; set; } = new List<string>();
    }

    public class GrowthOpportunityItem
    {
        public string Opportunity { get; set; } = string.Empty;
        public double PotentialRevenue { get; set; }
        public string Effort { get; set; } = string.Empty;
        public string Timeline { get; set; } = string.Empty;
    }
}
