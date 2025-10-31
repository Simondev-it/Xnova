using System;

namespace Xnova.API.RequestModel
{
    /// <summary>
    /// Request model cho các báo cáo Owner
    /// </summary>
    public class OwnerReportRequest
    {
        /// <summary>
        /// Owner ID (User ID có role Owner)
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        /// Period type: daily, weekly, monthly, quarterly, yearly, alltime, custom
        /// - daily: Hôm nay
        /// - weekly: 7 ngày gần nhất
        /// - monthly: Tháng hiện tại
        /// - quarterly: Quý hiện tại (3 tháng)
        /// - yearly: Năm hiện tại
        /// - alltime: Toàn bộ thời gian
        /// - custom: Tùy chỉnh (cần startDate, endDate)
        /// </summary>
        public string Period { get; set; } = "monthly";

        /// <summary>
        /// Ngày bắt đầu (YYYY-MM-DD) - Chỉ dùng khi Period = "custom"
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc (YYYY-MM-DD) - Chỉ dùng khi Period = "custom"
        /// </summary>
        public DateTime? EndDate { get; set; }
    }

    /// <summary>
    /// Request model cho export báo cáo
    /// </summary>
    public class ExportReportRequest
    {
        /// <summary>
        /// Owner ID
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        /// Report type: revenue, bookings, users, performance
        /// </summary>
        public string ReportType { get; set; } = "revenue";

        /// <summary>
        /// Export format: csv, pdf, excel
        /// </summary>
        public string Format { get; set; } = "csv";

        /// <summary>
        /// Ngày bắt đầu
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}
