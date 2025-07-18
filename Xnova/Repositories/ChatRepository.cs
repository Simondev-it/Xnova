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

            // Append history
            foreach (var (q, a) in history)
            {
                sb.AppendLine($"Q: {q}");
                sb.AppendLine($"A: {a}");
                sb.AppendLine();
            }

            // Append câu hỏi mới
            sb.AppendLine($"Q: {prompt}");
            sb.Append("A: ");

            string promptWithHistory = sb.ToString();

            // 3. Gửi promptWithHistory lên API Gemini
            string entityType = IdentifyEntity(prompt);
            object dbResult = entityType.ToLower() switch
            {
                "user" => _context.Users.Take(100).ToList(),
                "booking" => _context.Bookings.Take(100).ToList(),
                "pod" => _context.Fields.Take(100).ToList(),
                _ => "Không rõ câu hỏi liên quan đến dữ liệu nào."
            };

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

            // 4. Lưu câu hỏi + trả lời mới vào cache
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
    }
}