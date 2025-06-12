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
    }
}
