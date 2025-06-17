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
    public class SlotRepository : GenericRepository<Slot>
    {
        public SlotRepository(XnovaContext context) => _context = context;

        public async Task<List<Slot>> GetAllAsync()
        {
            return await _context.Slots.Include(p => p.BookingSlots).ToListAsync();
        }

        public async Task<Slot> GetByIdAsync(int id)
        {
            var result = await _context.Slots.Include(p => p.BookingSlots).FirstAsync(p => p.Id == id);

            return result;
        }
        //public async Task<List<Slot>> GetByBookingIdAsync(int bookingId)
        //{
        //    return await _context.Slots
        //        .Where(sb => sb.BookingId == bookingId)
        //        .ToListAsync();
        //}
    }
}
