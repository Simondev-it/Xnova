using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xnova.Models;

namespace Xnova.Repositories
{
    public class ChatRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "AIzaSyCLz5PRiSana5TSg3KOjjslqGlfTMt8tI0";
        private readonly XnovaContext _context;
        private readonly IMemoryCache _memoryCache;
        private const string HistoryPrefix = "ChatHistory_";
        private readonly string _defaultGreeting = "Xin chào! Tôi là trợ lý AI của bạn. Hãy đặt câu hỏi để tôi hỗ trợ nhé!";

        public ChatRepository(XnovaContext context, IMemoryCache memoryCache)
        {
            _httpClient = new HttpClient();
            _context = context;
            _memoryCache = memoryCache;
        }

        public string GetGreeting()
        {
            return _defaultGreeting;
        }

        public void SaveToHistory(string? userId, string question, string answer)
        {
            // Sử dụng sessionId cho guest nếu userId là null
            var key = HistoryPrefix + (userId ?? Guid.NewGuid().ToString());
            var history = _memoryCache.Get<List<(string Question, string Answer)>>(key) ?? new List<(string, string)>();
            history.Add((question, answer));
            _memoryCache.Set(key, history, TimeSpan.FromMinutes(30));

            Console.WriteLine($"[SaveToHistory] key={key} saved {history.Count} entries");
        }

        public List<(string Question, string Answer)> GetHistory(string? userId)
        {
            // Trả về lịch sử rỗng cho guest nếu không có userId hoặc sessionId
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("[GetHistory] No history for guest without session.");
                return new List<(string, string)>();
            }

            var key = HistoryPrefix + userId;
            var history = _memoryCache.Get<List<(string Question, string Answer)>>(key) ?? new List<(string, string)>();
            Console.WriteLine($"[GetHistory] key={key} retrieved {history.Count} entries");
            return history;
        }

        public async Task<(string Reply, string? SessionId)> AskAsync(string prompt, string? userId)
        {
            string? sessionId = null;
            if (string.IsNullOrEmpty(userId))
            {
                sessionId = Guid.NewGuid().ToString(); // Tạo sessionId cho guest
            }

            // 1. Lấy lịch sử chat hiện tại
            var history = GetHistory(userId ?? sessionId);

            // 2. Tạo đoạn prompt kèm ngữ cảnh (history)
            var sb = new StringBuilder();
            foreach (var (q, a) in history)
            {
                sb.AppendLine($"Q: {q}");
                sb.AppendLine($"A: {a}");
                sb.AppendLine();
            }

            sb.AppendLine($"Q: {prompt}");
            sb.Append("A: ");

            string promptWithHistory = sb.ToString();

            // 3. Xác định loại dữ liệu và lấy dữ liệu liên quan từ DB
            string entityType = IdentifyEntity(prompt);
            object dbResult;

            if (entityType.ToLower() == "booking")
            {
                if (int.TryParse(userId, out int parsedUserId))
                {
                    // Lấy danh sách booking của user có userId đó
                    dbResult = GetUserBookingHistory(parsedUserId);

                }
                else
                {
                    dbResult = "Không thể xác định người dùng để truy xuất lịch sử booking.";
                }
            }
            else
            {
                dbResult = entityType.ToLower() switch
                {
                    "user" => _context.Users.Take(100).ToList(),
                    "field" => _context.Fields.Take(100).ToList(),
                    _ => "Không rõ câu hỏi liên quan đến dữ liệu nào."
                };
            }

            // 4. Tạo prompt đầy đủ gồm dữ liệu + lịch sử
            var combinedPrompt = $"{promptWithHistory}\n\nDữ liệu liên quan:\n{JsonSerializer.Serialize(dbResult)}";

            var body = new
            {
                contents = new[]
                {
            new
            {
                parts = new[] { new { text = combinedPrompt } }
            }
        }
            };

            var json = JsonSerializer.Serialize(body);
            var request = new StringContent(json, Encoding.UTF8, "application/json");
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}";

            var response = await _httpClient.PostAsync(url, request);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                SaveToHistory(userId ?? sessionId, prompt, $"Error: {result}");
                return ($"Error: {result}", sessionId);
            }

            using var doc = JsonDocument.Parse(result);
            var reply = doc.RootElement
                           .GetProperty("candidates")[0]
                           .GetProperty("content")
                           .GetProperty("parts")[0]
                           .GetProperty("text")
                           .GetString();

            // 5. Lưu lại phản hồi mới vào cache
            SaveToHistory(userId ?? sessionId, prompt, reply ?? "Không có phản hồi.");

            return (reply ?? "Không có phản hồi.", sessionId);
        }


        private string IdentifyEntity(string prompt)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "user", "user" }, { "người dùng", "user" }, { "khách hàng", "user" },
                { "booking", "booking" }, { "đặt lịch", "booking" }, { "lịch đặt", "booking" },
                { "pod", "pod" }, { "phòng", "pod" }, { "chỗ ngồi", "pod" }
            };

            foreach (var entry in map)
            {
                if (prompt.Contains(entry.Key, StringComparison.OrdinalIgnoreCase))
                    return entry.Value;
            }

            return "unknown";
        }
        private object GetUserBookingHistory(int userId)
        {
            var bookings = _context.Bookings
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CurrentDate)
                .Take(10)
                .Select(b => new
                {
                    b.Id,
                    b.Date,
                    b.CurrentDate,
                    b.Feedback,
                    b.Rating,
                    b.Status,
                    FieldName = b.Field != null ? b.Field.Name : "Không có"
                })
                .ToList();

            return bookings;
        }


    }
}