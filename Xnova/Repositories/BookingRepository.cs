using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xnova.Base;
using Xnova.Models;

namespace Xnova.Repositories
{
    public class BookingRepository : GenericRepository<Booking>
    {
        public BookingRepository(XnovaContext context) => _context = context;

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.Bookings.Include(p=>p.BookingSlots).ToListAsync();
        }
        public async Task<Booking> GetByIdAsync(int id )
        {
            return await _context.Bookings.Include(p => p.BookingSlots).FirstAsync(p => p.Id == id);
        }
        public async Task RemoveAsync(Booking booking)
        {
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }
        public async Task SaveAsync() // 👈 Triển khai SaveAsync
        {
            await _context.SaveChangesAsync();
        }
        public async Task AddAsync(BookingSlot bookingSlot)
        {
            await _context.BookingSlots.AddAsync(bookingSlot);
        }

        /// <summary>
        /// Lấy bookings theo danh sách fieldIds và khoảng thời gian
        /// </summary>
        public async Task<List<Booking>> GetBookingsByFieldIdsAndDateRangeAsync(
            List<int> fieldIds, 
            DateOnly startDate, 
            DateOnly endDate)
        {
            return await _context.Bookings
                .Where(b => b.FieldId.HasValue 
                    && fieldIds.Contains(b.FieldId.Value)
                    && b.Date.HasValue
                    && b.Date.Value >= startDate
                    && b.Date.Value <= endDate)
                .Include(b => b.Field)
                    .ThenInclude(f => f.Venue)
                .Include(b => b.User)
                .Include(b => b.Payments)
                .Include(b => b.BookingSlots)
                    .ThenInclude(bs => bs.Slot)
                .ToListAsync();
        }

    }
}
