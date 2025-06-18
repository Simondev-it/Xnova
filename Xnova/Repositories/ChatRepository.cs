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

        public ChatRepository(XnovaContext context, IMemoryCache memoryCache)
        {
            _httpClient = new HttpClient();
            _context = context;
            _memoryCache = memoryCache;
        }

        public void SaveToHistory(string userId, string question, string answer)
        {
            var key = HistoryPrefix + userId;
            var history = _memoryCache.Get<List<(string Question, string Answer)>>(key) ?? new List<(string, string)>();
            history.Add((question, answer));
            _memoryCache.Set(key, history, TimeSpan.FromMinutes(30));

            Console.WriteLine($"[SaveToHistory] userId={userId} saved {history.Count} entries");
        }

        public List<(string Question, string Answer)> GetHistory(string userId)
        {
            var key = HistoryPrefix + userId;
            var history = _memoryCache.Get<List<(string Question, string Answer)>>(key) ?? new List<(string, string)>();
            Console.WriteLine($"[GetHistory] userId={userId} retrieved {history.Count} entries");
            return history;
        }

        public async Task<string> AskAsync(string prompt, string userId)
        {
            // 1. Lấy lịch sử chat hiện tại của user
            var history = GetHistory(userId); // List<(string Question, string Answer)>

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

            // Bạn có thể thêm phần dữ liệu liên quan vào prompt nếu cần
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
                return $"Error: {result}";

            using var doc = JsonDocument.Parse(result);
            var reply = doc.RootElement
                           .GetProperty("candidates")[0]
                           .GetProperty("content")
                           .GetProperty("parts")[0]
                           .GetProperty("text")
                           .GetString();

            // 4. Lưu câu hỏi + trả lời mới vào cache
            SaveToHistory(userId, prompt, reply ?? "Không có phản hồi.");

            return reply ?? "Không có phản hồi.";
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
