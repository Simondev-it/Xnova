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

    }
}
