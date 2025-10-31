using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xnova;
using Xnova.API.RequestModel;
using Xnova.API.ResponseModel;

namespace Xnova.API.Controllers
{
    /// <summary>
    /// Controller cho Owner Dashboard và Reports
    /// Quản lý tất cả các API endpoints liên quan đến dashboard, báo cáo và phân tích cho Owner
    /// </summary>
    [Route("api/owner")]
    [ApiController]
    [Authorize(Roles = "Owner,Admin")]
    public class OwnerDashboardController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public OwnerDashboardController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Dashboard Endpoints

        /// <summary>
        /// Lấy thống kê tổng quan dashboard cho owner
        /// GET: api/owner/dashboard/stats?ownerId={ownerId}
        /// </summary>
        /// <param name="ownerId">ID của owner (User có role Owner)</param>
        /// <returns>Dashboard statistics bao gồm revenue, users, bookings, fields</returns>
        [HttpGet("dashboard/stats")]
        [ProducesResponseType(typeof(ApiResponse<DashboardStatsResponse>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<ActionResult<ApiResponse<DashboardStatsResponse>>> GetDashboardStats([FromQuery] int ownerId)
        {
            try
            {
                if (ownerId <= 0)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Error = new ErrorDetail
                        {
                            Code = "INVALID_OWNER_ID",
                            Message = "Owner ID không hợp lệ"
                        }
                    });
                }

                var today = DateTime.Now.Date;
                var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                var startOfMonth = new DateTime(today.Year, today.Month, 1);

                // Lấy revenue data
                var todayRevenue = await GetRevenueByPeriodInternal(ownerId, today, today);
                var weekRevenue = await GetRevenueByPeriodInternal(ownerId, startOfWeek, today);
                var monthRevenue = await GetRevenueByPeriodInternal(ownerId, startOfMonth, today);

                // Lấy monthly revenue cho 6 tháng gần nhất
                var monthlyRevenue = new System.Collections.Generic.List<decimal>();
                for (int i = 5; i >= 0; i--)
                {
                    var monthStart = today.AddMonths(-i).AddDays(-today.Day + 1);
                    var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                    if (monthEnd > today) monthEnd = today;
                    
                    var revenue = await GetRevenueByPeriodInternal(ownerId, monthStart, monthEnd);
                    monthlyRevenue.Add(revenue.TotalRevenue);
                }

                // Lấy user data
                var userData = await GetUserDataInternal(ownerId, today, startOfWeek, startOfMonth);

                // Lấy booking data
                var bookingData = await GetBookingDataInternal(ownerId, today, startOfWeek);

                // Lấy field data
                var fieldData = await GetFieldDataInternal(ownerId, startOfMonth, today);

                var response = new DashboardStatsResponse
                {
                    Revenue = new RevenueData
                    {
                        Total = monthRevenue.TotalRevenue,
                        Today = todayRevenue.TotalRevenue,
                        Week = weekRevenue.TotalRevenue,
                        Month = monthRevenue.TotalRevenue,
                        Monthly = monthlyRevenue,
                        Change = monthRevenue.Change,
                        Trend = monthRevenue.Trend
                    },
                    Users = userData,
                    Bookings = bookingData,
                    Fields = fieldData
                };

                return Ok(new ApiResponse<DashboardStatsResponse>
                {
                    Success = true,
                    Data = response,
                    Message = "Lấy thống kê dashboard thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Error = new ErrorDetail
                    {
                        Code = "INTERNAL_ERROR",
                        Message = "Lỗi khi lấy thống kê dashboard",
                        Details = ex.Message
                    }
                });
            }
        }

        /// <summary>
        /// Lấy dữ liệu revenue theo period
        /// GET: api/owner/dashboard/revenue?ownerId={ownerId}&period={period}
        /// </summary>
        /// <param name="ownerId">ID của owner</param>
        /// <param name="period">Khoảng thời gian: today/daily, week/weekly, month/monthly, quarter/quarterly, year/yearly, alltime</param>
        /// <returns>Revenue data cho period được chọn</returns>
        [HttpGet("dashboard/revenue")]
        [ProducesResponseType(typeof(ApiResponse<RevenueByPeriodResponse>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        public async Task<ActionResult<ApiResponse<RevenueByPeriodResponse>>> GetRevenueByPeriod(
            [FromQuery] int ownerId,
            [FromQuery] string period = "today")
        {
            try
            {
                if (ownerId <= 0)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Error = new ErrorDetail
                        {
                            Code = "INVALID_OWNER_ID",
                            Message = "Owner ID không hợp lệ"
                        }
                    });
                }

                DateTime startDate, endDate;
                var today = DateTime.Now.Date;

                switch (period.ToLower())
                {
                    case "today":
                    case "daily":
                        startDate = today;
                        endDate = today;
                        break;
                    case "week":
                    case "weekly":
                        startDate = today.AddDays(-(int)today.DayOfWeek);
                        endDate = today;
                        break;
                    case "month":
                    case "monthly":
                        startDate = new DateTime(today.Year, today.Month, 1);
                        endDate = today;
                        break;
                    case "quarter":
                    case "quarterly":
                        var currentQuarter = (today.Month - 1) / 3;
                        startDate = new DateTime(today.Year, currentQuarter * 3 + 1, 1);
                        endDate = today;
                        break;
                    case "year":
                    case "yearly":
                        startDate = new DateTime(today.Year, 1, 1);
                        endDate = today;
                        break;
                    case "alltime":
                        startDate = new DateTime(2020, 1, 1);
                        endDate = today;
                        break;
                    default:
                        return BadRequest(new ApiErrorResponse
                        {
                            Error = new ErrorDetail
                            {
                                Code = "INVALID_PERIOD",
                                Message = "Period phải là: today/daily, week/weekly, month/monthly, quarter/quarterly, year/yearly, hoặc alltime"
                            }
                        });
                }

                var result = await GetRevenueByPeriodInternal(ownerId, startDate, endDate);

                return Ok(new ApiResponse<RevenueByPeriodResponse>
                {
                    Success = true,
                    Data = result,
                    Message = $"Lấy dữ liệu revenue {period} thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Error = new ErrorDetail
                    {
                        Code = "INTERNAL_ERROR",
                        Message = "Lỗi khi lấy dữ liệu revenue",
                        Details = ex.Message
                    }
                });
            }
        }

        /// <summary>
        /// Lấy dữ liệu bookings theo period
        /// GET: api/owner/dashboard/bookings?ownerId={ownerId}&period={period}
        /// </summary>
        /// <param name="ownerId">ID của owner</param>
        /// <param name="period">Khoảng thời gian: today/daily, week/weekly, month/monthly, quarter/quarterly, year/yearly, alltime</param>
        /// <returns>Booking data cho period được chọn</returns>
        [HttpGet("dashboard/bookings")]
        [ProducesResponseType(typeof(ApiResponse<BookingsByPeriodResponse>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        public async Task<ActionResult<ApiResponse<BookingsByPeriodResponse>>> GetBookingsByPeriod(
            [FromQuery] int ownerId,
            [FromQuery] string period = "today")
        {
            try
            {
                if (ownerId <= 0)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Error = new ErrorDetail
                        {
                            Code = "INVALID_OWNER_ID",
                            Message = "Owner ID không hợp lệ"
                        }
                    });
                }

                DateTime startDate, endDate;
                var today = DateTime.Now.Date;

                switch (period.ToLower())
                {
                    case "today":
                    case "daily":
                        startDate = today;
                        endDate = today;
                        break;
                    case "week":
                    case "weekly":
                        startDate = today.AddDays(-(int)today.DayOfWeek);
                        endDate = today;
                        break;
                    case "month":
                    case "monthly":
                        startDate = new DateTime(today.Year, today.Month, 1);
                        endDate = today;
                        break;
                    case "quarter":
                    case "quarterly":
                        var currentQuarter = (today.Month - 1) / 3;
                        startDate = new DateTime(today.Year, currentQuarter * 3 + 1, 1);
                        endDate = today;
                        break;
                    case "year":
                    case "yearly":
                        startDate = new DateTime(today.Year, 1, 1);
                        endDate = today;
                        break;
                    case "alltime":
                        startDate = new DateTime(2020, 1, 1);
                        endDate = today;
                        break;
                    default:
                        return BadRequest(new ApiErrorResponse
                        {
                            Error = new ErrorDetail
                            {
                                Code = "INVALID_PERIOD",
                                Message = "Period phải là: today/daily, week/weekly, month/monthly, quarter/quarterly, year/yearly, hoặc alltime"
                            }
                        });
                }

                var result = await GetBookingsByPeriodInternal(ownerId, startDate, endDate);

                return Ok(new ApiResponse<BookingsByPeriodResponse>
                {
                    Success = true,
                    Data = result,
                    Message = $"Lấy dữ liệu bookings {period} thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Error = new ErrorDetail
                    {
                        Code = "INTERNAL_ERROR",
                        Message = "Lỗi khi lấy dữ liệu bookings",
                        Details = ex.Message
                    }
                });
            }
        }

        /// <summary>
        /// Lấy dữ liệu users theo period
        /// GET: api/owner/dashboard/users?ownerId={ownerId}&period={period}
        /// </summary>
        /// <param name="ownerId">ID của owner</param>
        /// <param name="period">Khoảng thời gian: today/daily, week/weekly, month/monthly, quarter/quarterly, year/yearly, alltime</param>
        /// <returns>User data cho period được chọn</returns>
        [HttpGet("dashboard/users")]
        [ProducesResponseType(typeof(ApiResponse<UsersByPeriodResponse>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        public async Task<ActionResult<ApiResponse<UsersByPeriodResponse>>> GetUsersByPeriod(
            [FromQuery] int ownerId,
            [FromQuery] string period = "today")
        {
            try
            {
                if (ownerId <= 0)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Error = new ErrorDetail
                        {
                            Code = "INVALID_OWNER_ID",
                            Message = "Owner ID không hợp lệ"
                        }
                    });
                }

                DateTime startDate, endDate;
                var today = DateTime.Now.Date;

                switch (period.ToLower())
                {
                    case "today":
                    case "daily":
                        startDate = today;
                        endDate = today;
                        break;
                    case "week":
                    case "weekly":
                        startDate = today.AddDays(-(int)today.DayOfWeek);
                        endDate = today;
                        break;
                    case "month":
                    case "monthly":
                        startDate = new DateTime(today.Year, today.Month, 1);
                        endDate = today;
                        break;
                    case "quarter":
                    case "quarterly":
                        var currentQuarter = (today.Month - 1) / 3;
                        startDate = new DateTime(today.Year, currentQuarter * 3 + 1, 1);
                        endDate = today;
                        break;
                    case "year":
                    case "yearly":
                        startDate = new DateTime(today.Year, 1, 1);
                        endDate = today;
                        break;
                    case "alltime":
                        startDate = new DateTime(2020, 1, 1);
                        endDate = today;
                        break;
                    default:
                        return BadRequest(new ApiErrorResponse
                        {
                            Error = new ErrorDetail
                            {
                                Code = "INVALID_PERIOD",
                                Message = "Period phải là: today/daily, week/weekly, month/monthly, quarter/quarterly, year/yearly, hoặc alltime"
                            }
                        });
                }

                var result = await GetUsersByPeriodInternal(ownerId, startDate, endDate);

                return Ok(new ApiResponse<UsersByPeriodResponse>
                {
                    Success = true,
                    Data = result,
                    Message = $"Lấy dữ liệu users {period} thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Error = new ErrorDetail
                    {
                        Code = "INTERNAL_ERROR",
                        Message = "Lỗi khi lấy dữ liệu users",
                        Details = ex.Message
                    }
                });
            }
        }

        /// <summary>
        /// Lấy danh sách top fields theo bookings hoặc revenue
        /// GET: api/owner/dashboard/top-fields?ownerId={ownerId}&limit={limit}&sortBy={sortBy}
        /// </summary>
        /// <param name="ownerId">ID của owner</param>
        /// <param name="limit">Số lượng fields cần lấy (default: 10)</param>
        /// <param name="sortBy">Sắp xếp theo: bookings hoặc revenue (default: revenue)</param>
        /// <returns>Danh sách top fields</returns>
        [HttpGet("dashboard/top-fields")]
        [ProducesResponseType(typeof(ApiResponse<System.Collections.Generic.List<TopFieldItem>>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        public async Task<ActionResult<ApiResponse<System.Collections.Generic.List<TopFieldItem>>>> GetTopFields(
            [FromQuery] int ownerId,
            [FromQuery] int limit = 10,
            [FromQuery] string sortBy = "revenue")
        {
            try
            {
                if (ownerId <= 0)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Error = new ErrorDetail
                        {
                            Code = "INVALID_OWNER_ID",
                            Message = "Owner ID không hợp lệ"
                        }
                    });
                }

                var today = DateTime.Now.Date;
                var startOfMonth = new DateTime(today.Year, today.Month, 1);
                
                var fieldData = await GetFieldDataInternal(ownerId, startOfMonth, today);
                
                var topFields = sortBy.ToLower() == "bookings"
                    ? fieldData.TopFields.OrderByDescending(f => f.Bookings).Take(limit).ToList()
                    : fieldData.TopFields.OrderByDescending(f => f.Revenue).Take(limit).ToList();

                return Ok(new ApiResponse<System.Collections.Generic.List<TopFieldItem>>
                {
                    Success = true,
                    Data = topFields,
                    Message = "Lấy top fields thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Error = new ErrorDetail
                    {
                        Code = "INTERNAL_ERROR",
                        Message = "Lỗi khi lấy top fields",
                        Details = ex.Message
                    }
                });
            }
        }

        #endregion

        #region Report Endpoints

        /// <summary>
        /// Lấy báo cáo revenue chi tiết
        /// GET: api/owner/reports/revenue?ownerId={ownerId}&period={period}
        /// GET: api/owner/reports/revenue?ownerId={ownerId}&period=custom&startDate={startDate}&endDate={endDate}
        /// </summary>
        /// <param name="request">Request model chứa ownerId, period, startDate (optional), endDate (optional)</param>
        /// <returns>Báo cáo revenue chi tiết</returns>
        [HttpGet("reports/revenue")]
        [ProducesResponseType(typeof(ApiResponse<RevenueReportResponse>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        public async Task<ActionResult<ApiResponse<RevenueReportResponse>>> GetRevenueReport([FromQuery] OwnerReportRequest request)
        {
            try
            {
                if (request.OwnerId <= 0)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Error = new ErrorDetail
                        {
                            Code = "INVALID_OWNER_ID",
                            Message = "Owner ID không hợp lệ"
                        }
                    });
                }

                // Tính toán startDate và endDate dựa trên period
                var (startDate, endDate) = CalculateDateRangeFromPeriod(request.Period, request.StartDate, request.EndDate);

                if (startDate == DateTime.MinValue || endDate == DateTime.MinValue)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Error = new ErrorDetail
                        {
                            Code = "INVALID_PERIOD",
                            Message = "Period không hợp lệ hoặc thiếu startDate/endDate cho period custom"
                        }
                    });
                }

                if (startDate > endDate)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Error = new ErrorDetail
                        {
                            Code = "INVALID_DATE_RANGE",
                            Message = "StartDate phải nhỏ hơn hoặc bằng EndDate"
                        }
                    });
                }

                var result = await _unitOfWork.OwnerReportRepository.GetRevenueReportAsync(
                    request.OwnerId,
                    startDate,
                    endDate
                );

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Data = result,
                    Message = "Lấy báo cáo revenue thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Error = new ErrorDetail
                    {
                        Code = "INTERNAL_ERROR",
                        Message = "Lỗi khi lấy báo cáo revenue",
                        Details = ex.Message
                    }
                });
            }
        }

        /// <summary>
        /// Lấy báo cáo bookings chi tiết
        /// GET: api/owner/reports/bookings?ownerId={ownerId}&period={period}
        /// GET: api/owner/reports/bookings?ownerId={ownerId}&period=custom&startDate={startDate}&endDate={endDate}
        /// </summary>
        /// <param name="request">Request model chứa ownerId, period, startDate (optional), endDate (optional)</param>
        /// <returns>Báo cáo bookings chi tiết</returns>
        [HttpGet("reports/bookings")]
        [ProducesResponseType(typeof(ApiResponse<BookingReportResponse>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        public async Task<ActionResult<ApiResponse<BookingReportResponse>>> GetBookingReport([FromQuery] OwnerReportRequest request)
        {
            try
            {
                if (request.OwnerId <= 0)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Error = new ErrorDetail
                        {
                            Code = "INVALID_OWNER_ID",
                            Message = "Owner ID không hợp lệ"
                        }
                    });
                }

                // Tính toán startDate và endDate dựa trên period
                var (startDate, endDate) = CalculateDateRangeFromPeriod(request.Period, request.StartDate, request.EndDate);

                if (startDate == DateTime.MinValue || endDate == DateTime.MinValue)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Error = new ErrorDetail
                        {
                            Code = "INVALID_PERIOD",
                            Message = "Period không hợp lệ hoặc thiếu startDate/endDate cho period custom"
                        }
                    });
                }

                if (startDate > endDate)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Error = new ErrorDetail
                        {
                            Code = "INVALID_DATE_RANGE",
                            Message = "StartDate phải nhỏ hơn hoặc bằng EndDate"
                        }
                    });
                }

                var result = await _unitOfWork.OwnerReportRepository.GetBookingReportAsync(
                    request.OwnerId,
                    startDate,
                    endDate
                );

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Data = result,
                    Message = "Lấy báo cáo bookings thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Error = new ErrorDetail
                    {
                        Code = "INTERNAL_ERROR",
                        Message = "Lỗi khi lấy báo cáo bookings",
                        Details = ex.Message
                    }
                });
            }
        }

        /// <summary>
        /// Lấy báo cáo users chi tiết
        /// GET: api/owner/reports/users?ownerId={ownerId}&period={period}
        /// GET: api/owner/reports/users?ownerId={ownerId}&period=custom&startDate={startDate}&endDate={endDate}
        /// </summary>
        /// <param name="request">Request model chứa ownerId, period, startDate (optional), endDate (optional)</param>
        /// <returns>Báo cáo users chi tiết</returns>
        [HttpGet("reports/users")]
        [ProducesResponseType(typeof(ApiResponse<UserReportResponse>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        public async Task<ActionResult<ApiResponse<UserReportResponse>>> GetUserReport([FromQuery] OwnerReportRequest request)
        {
            try
            {
                if (request.OwnerId <= 0)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Error = new ErrorDetail
                        {
                            Code = "INVALID_OWNER_ID",
                            Message = "Owner ID không hợp lệ"
                        }
                    });
                }

                // Tính toán startDate và endDate dựa trên period
                var (startDate, endDate) = CalculateDateRangeFromPeriod(request.Period, request.StartDate, request.EndDate);

                if (startDate == DateTime.MinValue || endDate == DateTime.MinValue)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Error = new ErrorDetail
                        {
                            Code = "INVALID_PERIOD",
                            Message = "Period không hợp lệ hoặc thiếu startDate/endDate cho period custom"
                        }
                    });
                }

                if (startDate > endDate)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Error = new ErrorDetail
                        {
                            Code = "INVALID_DATE_RANGE",
                            Message = "StartDate phải nhỏ hơn hoặc bằng EndDate"
                        }
                    });
                }

                var result = await _unitOfWork.OwnerReportRepository.GetUserReportAsync(
                    request.OwnerId,
                    startDate,
                    endDate
                );

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Data = result,
                    Message = "Lấy báo cáo users thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Error = new ErrorDetail
                    {
                        Code = "INTERNAL_ERROR",
                        Message = "Lỗi khi lấy báo cáo users",
                        Details = ex.Message
                    }
                });
            }
        }

        /// <summary>
        /// Lấy báo cáo performance chi tiết
        /// GET: api/owner/reports/performance?ownerId={ownerId}&period={period}
        /// GET: api/owner/reports/performance?ownerId={ownerId}&period=custom&startDate={startDate}&endDate={endDate}
        /// </summary>
        /// <param name="request">Request model chứa ownerId, period, startDate (optional), endDate (optional)</param>
        /// <returns>Báo cáo performance chi tiết</returns>
        [HttpGet("reports/performance")]
        [ProducesResponseType(typeof(ApiResponse<PerformanceReportResponse>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        public async Task<ActionResult<ApiResponse<PerformanceReportResponse>>> GetPerformanceReport([FromQuery] OwnerReportRequest request)
        {
            try
            {
                if (request.OwnerId <= 0)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Error = new ErrorDetail
                        {
                            Code = "INVALID_OWNER_ID",
                            Message = "Owner ID không hợp lệ"
                        }
                    });
                }

                // Tính toán startDate và endDate dựa trên period
                var (startDate, endDate) = CalculateDateRangeFromPeriod(request.Period, request.StartDate, request.EndDate);

                if (startDate == DateTime.MinValue || endDate == DateTime.MinValue)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Error = new ErrorDetail
                        {
                            Code = "INVALID_PERIOD",
                            Message = "Period không hợp lệ hoặc thiếu startDate/endDate cho period custom"
                        }
                    });
                }

                if (startDate > endDate)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Error = new ErrorDetail
                        {
                            Code = "INVALID_DATE_RANGE",
                            Message = "StartDate phải nhỏ hơn hoặc bằng EndDate"
                        }
                    });
                }

                var result = await _unitOfWork.OwnerReportRepository.GetPerformanceReportAsync(
                    request.OwnerId,
                    startDate,
                    endDate
                );

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Data = result,
                    Message = "Lấy báo cáo performance thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Error = new ErrorDetail
                    {
                        Code = "INTERNAL_ERROR",
                        Message = "Lỗi khi lấy báo cáo performance",
                        Details = ex.Message
                    }
                });
            }
        }

        /// <summary>
        /// Export báo cáo (CSV, PDF)
        /// POST: api/owner/reports/export
        /// </summary>
        /// <param name="request">Request model chứa thông tin export</param>
        /// <returns>File báo cáo theo format được chọn</returns>
        [HttpPost("reports/export")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        public async Task<IActionResult> ExportReport([FromBody] ExportReportRequest request)
        {
            try
            {
                if (request.OwnerId <= 0)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Error = new ErrorDetail
                        {
                            Code = "INVALID_OWNER_ID",
                            Message = "Owner ID không hợp lệ"
                        }
                    });
                }

                // Lấy dữ liệu báo cáo
                object reportData;
                string fileName;

                switch (request.ReportType.ToLower())
                {
                    case "revenue":
                        reportData = await _unitOfWork.OwnerReportRepository.GetRevenueReportAsync(
                            request.OwnerId, request.StartDate, request.EndDate);
                        fileName = $"Revenue_Report_{DateTime.Now:yyyyMMdd}";
                        break;
                    case "bookings":
                        reportData = await _unitOfWork.OwnerReportRepository.GetBookingReportAsync(
                            request.OwnerId, request.StartDate, request.EndDate);
                        fileName = $"Booking_Report_{DateTime.Now:yyyyMMdd}";
                        break;
                    case "users":
                        reportData = await _unitOfWork.OwnerReportRepository.GetUserReportAsync(
                            request.OwnerId, request.StartDate, request.EndDate);
                        fileName = $"User_Report_{DateTime.Now:yyyyMMdd}";
                        break;
                    case "performance":
                        reportData = await _unitOfWork.OwnerReportRepository.GetPerformanceReportAsync(
                            request.OwnerId, request.StartDate, request.EndDate);
                        fileName = $"Performance_Report_{DateTime.Now:yyyyMMdd}";
                        break;
                    default:
                        return BadRequest(new ApiErrorResponse
                        {
                            Error = new ErrorDetail
                            {
                                Code = "INVALID_REPORT_TYPE",
                                Message = "Report type phải là: revenue, bookings, users, hoặc performance"
                            }
                        });
                }

                // Export dựa theo format
                byte[] fileContent;
                string contentType;

                switch (request.Format.ToLower())
                {
                    case "csv":
                        fileContent = ExportToCsv(reportData);
                        contentType = "text/csv";
                        fileName += ".csv";
                        break;
                    case "pdf":
                        fileContent = ExportToPdf(reportData);
                        contentType = "application/pdf";
                        fileName += ".pdf";
                        break;
                    case "excel":
                        fileContent = ExportToExcel(reportData);
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        fileName += ".xlsx";
                        break;
                    default:
                        return BadRequest(new ApiErrorResponse
                        {
                            Error = new ErrorDetail
                            {
                                Code = "INVALID_FORMAT",
                                Message = "Format phải là: csv, pdf, hoặc excel"
                            }
                        });
                }

                return File(fileContent, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Error = new ErrorDetail
                    {
                        Code = "INTERNAL_ERROR",
                        Message = "Lỗi khi export báo cáo",
                        Details = ex.Message
                    }
                });
            }
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Tính toán khoảng thời gian từ period type
        /// </summary>
        private (DateTime startDate, DateTime endDate) CalculateDateRangeFromPeriod(string period, DateTime? customStartDate, DateTime? customEndDate)
        {
            var today = DateTime.Now.Date;
            
            switch (period.ToLower())
            {
                case "daily":
                    // Hôm nay
                    return (today, today);
                
                case "weekly":
                    // 7 ngày gần nhất
                    return (today.AddDays(-6), today);
                
                case "monthly":
                    // Tháng hiện tại
                    var startOfMonth = new DateTime(today.Year, today.Month, 1);
                    return (startOfMonth, today);
                
                case "quarterly":
                    // Quý hiện tại (3 tháng)
                    var currentQuarter = (today.Month - 1) / 3;
                    var startOfQuarter = new DateTime(today.Year, currentQuarter * 3 + 1, 1);
                    return (startOfQuarter, today);
                
                case "yearly":
                    // Năm hiện tại
                    var startOfYear = new DateTime(today.Year, 1, 1);
                    return (startOfYear, today);
                
                case "alltime":
                    // Toàn bộ thời gian (từ 2020-01-01 đến hôm nay)
                    return (new DateTime(2020, 1, 1), today);
                
                case "custom":
                    // Tùy chỉnh - cần startDate và endDate
                    if (customStartDate.HasValue && customEndDate.HasValue)
                    {
                        return (customStartDate.Value.Date, customEndDate.Value.Date);
                    }
                    // Trả về giá trị invalid nếu thiếu dates
                    return (DateTime.MinValue, DateTime.MinValue);
                
                default:
                    // Period không hợp lệ
                    return (DateTime.MinValue, DateTime.MinValue);
            }
        }

        private async Task<RevenueByPeriodResponse> GetRevenueByPeriodInternal(int ownerId, DateTime startDate, DateTime endDate)
        {
            var venues = await _unitOfWork.VenueRepository.GetVenuesByOwnerIdAsync(ownerId);
            var fieldIds = venues.SelectMany(v => v.Fields ?? new System.Collections.Generic.List<Xnova.Models.Field>())
                .Select(f => f.Id).ToList();

            var bookings = await _unitOfWork.BookingRepository.GetBookingsByFieldIdsAndDateRangeAsync(
                fieldIds, DateOnly.FromDateTime(startDate), DateOnly.FromDateTime(endDate));

            var totalRevenue = bookings
                .Where(b => b.Status == 1)
                .SelectMany(b => b.Payments ?? new System.Collections.Generic.List<Xnova.Models.Payment>())
                .Where(p => p.Status == 1)
                .Sum(p => p.Amount ?? 0);

            // Calculate previous period
            var periodDays = (endDate - startDate).Days + 1;
            var previousStart = startDate.AddDays(-periodDays);
            var previousEnd = startDate.AddDays(-1);

            var previousBookings = await _unitOfWork.BookingRepository.GetBookingsByFieldIdsAndDateRangeAsync(
                fieldIds, DateOnly.FromDateTime(previousStart), DateOnly.FromDateTime(previousEnd));

            var previousRevenue = previousBookings
                .Where(b => b.Status == 1)
                .SelectMany(b => b.Payments ?? new System.Collections.Generic.List<Xnova.Models.Payment>())
                .Where(p => p.Status == 1)
                .Sum(p => p.Amount ?? 0);

            var change = previousRevenue > 0 
                ? (double)((totalRevenue - previousRevenue) / previousRevenue * 100) 
                : 0;

            var timeSeriesData = bookings
                .Where(b => b.Status == 1 && b.Date.HasValue)
                .GroupBy(b => b.Date!.Value)
                .Select(g => new DailyRevenueItem
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Revenue = g.SelectMany(b => b.Payments ?? new System.Collections.Generic.List<Xnova.Models.Payment>())
                        .Where(p => p.Status == 1)
                        .Sum(p => p.Amount ?? 0),
                    Bookings = g.Count()
                })
                .OrderBy(d => d.Date)
                .ToList();

            var fieldBreakdown = bookings
                .Where(b => b.Status == 1 && b.Field != null)
                .GroupBy(b => b.Field!)
                .Select(g => new FieldRevenueItem
                {
                    FieldId = g.Key.Id.ToString(),
                    FieldName = g.Key.Name ?? "Unknown",
                    Location = g.Key.Venue?.Address ?? "N/A",
                    Revenue = g.SelectMany(b => b.Payments ?? new System.Collections.Generic.List<Xnova.Models.Payment>())
                        .Where(p => p.Status == 1)
                        .Sum(p => p.Amount ?? 0),
                    Percentage = totalRevenue > 0 
                        ? (double)(g.SelectMany(b => b.Payments ?? new System.Collections.Generic.List<Xnova.Models.Payment>())
                            .Where(p => p.Status == 1)
                            .Sum(p => p.Amount ?? 0) / totalRevenue * 100) 
                        : 0,
                    Bookings = g.Count()
                })
                .OrderByDescending(f => f.Revenue)
                .ToList();

            return new RevenueByPeriodResponse
            {
                Period = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
                TotalRevenue = totalRevenue,
                PreviousPeriodRevenue = previousRevenue,
                Change = Math.Round(change, 2),
                Trend = change > 0 ? "up" : change < 0 ? "down" : "stable",
                TimeSeriesData = timeSeriesData,
                FieldBreakdown = fieldBreakdown
            };
        }

        private async Task<UserData> GetUserDataInternal(int ownerId, DateTime today, DateTime startOfWeek, DateTime startOfMonth)
        {
            var venues = await _unitOfWork.VenueRepository.GetVenuesByOwnerIdAsync(ownerId);
            var fieldIds = venues.SelectMany(v => v.Fields ?? new System.Collections.Generic.List<Xnova.Models.Field>())
                .Select(f => f.Id).ToList();

            var allBookings = await _unitOfWork.BookingRepository.GetAllAsync();
            var relevantBookings = allBookings.Where(b => b.FieldId.HasValue && fieldIds.Contains(b.FieldId.Value)).ToList();

            var monthlyUsers = relevantBookings
                .Where(b => b.Date.HasValue && b.Date.Value >= DateOnly.FromDateTime(startOfMonth))
                .Select(b => b.UserId)
                .Distinct()
                .Count();

            var weeklyUsers = relevantBookings
                .Where(b => b.Date.HasValue && b.Date.Value >= DateOnly.FromDateTime(startOfWeek))
                .Select(b => b.UserId)
                .Distinct()
                .Count();

            var dailyUsers = relevantBookings
                .Where(b => b.Date.HasValue && b.Date.Value == DateOnly.FromDateTime(today))
                .Select(b => b.UserId)
                .Distinct()
                .Count();

            // Calculate new users (first time bookers in the month)
            var newUsers = relevantBookings
                .Where(b => b.UserId.HasValue && b.Date.HasValue)
                .GroupBy(b => b.UserId)
                .Count(g => g.OrderBy(b => b.Date).First().Date >= DateOnly.FromDateTime(startOfMonth));

            return new UserData
            {
                Total = monthlyUsers,
                NewUsers = newUsers,
                Daily = dailyUsers,
                Weekly = weeklyUsers,
                Change = 0, // Could calculate month-over-month change
                Trend = "stable"
            };
        }

        private async Task<BookingData> GetBookingDataInternal(int ownerId, DateTime today, DateTime startOfWeek)
        {
            var venues = await _unitOfWork.VenueRepository.GetVenuesByOwnerIdAsync(ownerId);
            var fieldIds = venues.SelectMany(v => v.Fields ?? new System.Collections.Generic.List<Xnova.Models.Field>())
                .Select(f => f.Id).ToList();

            var allBookings = await _unitOfWork.BookingRepository.GetAllAsync();
            var relevantBookings = allBookings.Where(b => b.FieldId.HasValue && fieldIds.Contains(b.FieldId.Value)).ToList();

            var totalBookings = relevantBookings.Count;

            // Get last 7 days bookings
            var dailyBookings = new System.Collections.Generic.List<int>();
            for (int i = 6; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                var count = relevantBookings.Count(b => b.Date.HasValue && b.Date.Value == DateOnly.FromDateTime(date));
                dailyBookings.Add(count);
            }

            var bookingActivity = relevantBookings
                .Where(b => b.Date.HasValue && b.Date.Value >= DateOnly.FromDateTime(startOfWeek))
                .GroupBy(b => b.Date!.Value)
                .Select(g => new BookingActivityItem
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Bookings = g.Count(),
                    Revenue = g.Where(b => b.Status == 1)
                        .SelectMany(b => b.Payments ?? new System.Collections.Generic.List<Xnova.Models.Payment>())
                        .Where(p => p.Status == 1)
                        .Sum(p => p.Amount ?? 0),
                    AverageBookingValue = g.Where(b => b.Status == 1).Any()
                        ? (double)(g.Where(b => b.Status == 1)
                            .SelectMany(b => b.Payments ?? new System.Collections.Generic.List<Xnova.Models.Payment>())
                            .Where(p => p.Status == 1)
                            .Sum(p => p.Amount ?? 0) / g.Count(b => b.Status == 1))
                        : 0
                })
                .OrderBy(a => a.Date)
                .ToList();

            return new BookingData
            {
                Total = totalBookings,
                Daily = dailyBookings,
                Activity = bookingActivity,
                Change = 0,
                Trend = "stable"
            };
        }

        private async Task<FieldData> GetFieldDataInternal(int ownerId, DateTime startDate, DateTime endDate)
        {
            var venues = await _unitOfWork.VenueRepository.GetVenuesByOwnerIdAsync(ownerId);
            var allFields = venues.SelectMany(v => v.Fields ?? new System.Collections.Generic.List<Xnova.Models.Field>()).ToList();
            
            var fieldIds = allFields.Select(f => f.Id).ToList();
            var bookings = await _unitOfWork.BookingRepository.GetBookingsByFieldIdsAndDateRangeAsync(
                fieldIds, DateOnly.FromDateTime(startDate), DateOnly.FromDateTime(endDate));

            var topFields = allFields.Select(field =>
            {
                var fieldBookings = bookings.Where(b => b.FieldId == field.Id).ToList();
                var revenue = fieldBookings
                    .Where(b => b.Status == 1)
                    .SelectMany(b => b.Payments ?? new System.Collections.Generic.List<Xnova.Models.Payment>())
                    .Where(p => p.Status == 1)
                    .Sum(p => p.Amount ?? 0);

                var ratings = fieldBookings.Where(b => b.Rating.HasValue).Select(b => b.Rating!.Value).ToList();

                // Get average price from slots
                var slotsWithPrice = field.Slots?.Where(s => s.Price.HasValue).ToList() ?? new System.Collections.Generic.List<Xnova.Models.Slot>();
                var avgPrice = slotsWithPrice.Any() 
                    ? slotsWithPrice.Average(s => s.Price!.Value) 
                    : 0;

                // Map status: 0 = Active, 1 = Hidden, 2 = Under Maintenance (assumption based on common patterns)
                string statusName = field.Status switch
                {
                    0 => "Active",
                    1 => "Hidden",
                    2 => "Under Maintenance",
                    _ => "Unknown"
                };

                return new TopFieldItem
                {
                    Id = field.Id.ToString(),
                    Name = field.Name ?? "Unknown",
                    Location = field.Venue?.Address ?? "N/A",
                    Status = statusName,
                    Bookings = fieldBookings.Count,
                    Revenue = revenue,
                    PricePerHour = (decimal)avgPrice,
                    Description = field.Description ?? "",
                    IsVisible = field.Status == 0, // 0 = Active/Visible
                    Rating = ratings.Any() ? Math.Round(ratings.Average(), 2) : null,
                    Reviews = ratings.Count
                };
            })
            .OrderByDescending(f => f.Revenue)
            .ToList();

            return new FieldData
            {
                Total = allFields.Count,
                Active = allFields.Count(f => f.Status == 0), // 0 = Active
                Hidden = allFields.Count(f => f.Status == 1), // 1 = Hidden
                Maintenance = allFields.Count(f => f.Status == 2), // 2 = Under Maintenance
                TopFields = topFields
            };
        }

        private async Task<BookingsByPeriodResponse> GetBookingsByPeriodInternal(int ownerId, DateTime startDate, DateTime endDate)
        {
            var venues = await _unitOfWork.VenueRepository.GetVenuesByOwnerIdAsync(ownerId);
            var fieldIds = venues.SelectMany(v => v.Fields ?? new System.Collections.Generic.List<Xnova.Models.Field>())
                .Select(f => f.Id).ToList();

            var bookings = await _unitOfWork.BookingRepository.GetBookingsByFieldIdsAndDateRangeAsync(
                fieldIds, DateOnly.FromDateTime(startDate), DateOnly.FromDateTime(endDate));

            var totalBookings = bookings.Count;
            var confirmedBookings = bookings.Count(b => b.Status == 1);
            var cancelledBookings = bookings.Count(b => b.Status == 2);
            var pendingBookings = bookings.Count(b => b.Status == 0);

            var activity = bookings
                .Where(b => b.Date.HasValue)
                .GroupBy(b => b.Date!.Value)
                .Select(g => new BookingActivityItem
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Bookings = g.Count(),
                    Revenue = g.Where(b => b.Status == 1)
                        .SelectMany(b => b.Payments ?? new System.Collections.Generic.List<Xnova.Models.Payment>())
                        .Where(p => p.Status == 1)
                        .Sum(p => p.Amount ?? 0),
                    AverageBookingValue = g.Where(b => b.Status == 1).Any()
                        ? (double)(g.Where(b => b.Status == 1)
                            .SelectMany(b => b.Payments ?? new System.Collections.Generic.List<Xnova.Models.Payment>())
                            .Where(p => p.Status == 1)
                            .Sum(p => p.Amount ?? 0) / g.Count(b => b.Status == 1))
                        : 0
                })
                .OrderBy(a => a.Date)
                .ToList();

            var fieldBreakdown = bookings
                .Where(b => b.Field != null)
                .GroupBy(b => b.Field!)
                .Select(g => new FieldBookingItem
                {
                    FieldId = g.Key.Id.ToString(),
                    FieldName = g.Key.Name ?? "Unknown",
                    Bookings = g.Count(),
                    Percentage = totalBookings > 0 ? (double)g.Count() / totalBookings * 100 : 0,
                    OccupancyRate = 0 // Would need more complex calculation
                })
                .OrderByDescending(f => f.Bookings)
                .ToList();

            return new BookingsByPeriodResponse
            {
                Period = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
                TotalBookings = totalBookings,
                ConfirmedBookings = confirmedBookings,
                CancelledBookings = cancelledBookings,
                PendingBookings = pendingBookings,
                CancellationRate = totalBookings > 0 ? Math.Round((double)cancelledBookings / totalBookings * 100, 2) : 0,
                Activity = activity,
                FieldBreakdown = fieldBreakdown
            };
        }

        private async Task<UsersByPeriodResponse> GetUsersByPeriodInternal(int ownerId, DateTime startDate, DateTime endDate)
        {
            var venues = await _unitOfWork.VenueRepository.GetVenuesByOwnerIdAsync(ownerId);
            var fieldIds = venues.SelectMany(v => v.Fields ?? new System.Collections.Generic.List<Xnova.Models.Field>())
                .Select(f => f.Id).ToList();

            var allBookings = await _unitOfWork.BookingRepository.GetAllAsync();
            var relevantBookings = allBookings.Where(b => b.FieldId.HasValue && fieldIds.Contains(b.FieldId.Value)).ToList();

            var periodBookings = relevantBookings
                .Where(b => b.Date.HasValue 
                    && b.Date.Value >= DateOnly.FromDateTime(startDate) 
                    && b.Date.Value <= DateOnly.FromDateTime(endDate))
                .ToList();

            var activeUsers = periodBookings.Select(b => b.UserId).Distinct().Count();

            var newUsers = relevantBookings
                .Where(b => b.UserId.HasValue && b.Date.HasValue)
                .GroupBy(b => b.UserId)
                .Count(g => g.OrderBy(b => b.Date).First().Date >= DateOnly.FromDateTime(startDate) 
                    && g.OrderBy(b => b.Date).First().Date <= DateOnly.FromDateTime(endDate));

            // Calculate previous period users
            var periodDays = (endDate - startDate).Days + 1;
            var previousStart = startDate.AddDays(-periodDays);
            var previousEnd = startDate.AddDays(-1);

            var previousUsers = relevantBookings
                .Where(b => b.Date.HasValue 
                    && b.Date.Value >= DateOnly.FromDateTime(previousStart) 
                    && b.Date.Value < DateOnly.FromDateTime(startDate))
                .Select(b => b.UserId)
                .Distinct()
                .ToList();

            var currentUsers = periodBookings.Select(b => b.UserId).Distinct().ToList();
            var returningUsers = currentUsers.Count(u => previousUsers.Contains(u));

            var userGrowth = periodBookings
                .Where(b => b.Date.HasValue)
                .GroupBy(b => b.Date!.Value)
                .Select(g => new UserGrowthItem
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    NewUsers = relevantBookings
                        .Where(b => b.UserId.HasValue)
                        .GroupBy(b => b.UserId)
                        .Count(ug => ug.OrderBy(b => b.Date).First().Date == g.Key),
                    TotalUsers = relevantBookings
                        .Where(b => b.Date.HasValue && b.Date.Value <= g.Key)
                        .Select(b => b.UserId)
                        .Distinct()
                        .Count(),
                    ActiveUsers = g.Select(b => b.UserId).Distinct().Count()
                })
                .OrderBy(u => u.Date)
                .ToList();

            return new UsersByPeriodResponse
            {
                Period = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
                TotalUsers = activeUsers,
                NewUsers = newUsers,
                ActiveUsers = activeUsers,
                ReturningUsers = returningUsers,
                RetentionRate = previousUsers.Count > 0 
                    ? Math.Round((double)returningUsers / previousUsers.Count * 100, 2) 
                    : 0,
                UserGrowth = userGrowth
            };
        }

        private byte[] ExportToCsv(object data)
        {
            // Simplified CSV export - would need proper CSV library
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            return System.Text.Encoding.UTF8.GetBytes(json);
        }

        private byte[] ExportToPdf(object data)
        {
            // Simplified PDF export - would need PDF library like iTextSharp
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            return System.Text.Encoding.UTF8.GetBytes(json);
        }

        private byte[] ExportToExcel(object data)
        {
            // Simplified Excel export - would need Excel library like EPPlus or ClosedXML
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            return System.Text.Encoding.UTF8.GetBytes(json);
        }

        #endregion
    }
}
