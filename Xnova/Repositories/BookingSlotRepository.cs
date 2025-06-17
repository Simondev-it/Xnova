using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xnova.Base;
using Xnova.Models;


namespace Xnova.Repositories
{
    public class BookingSlotRepository : GenericRepository<BookingSlot>
    {
        public BookingSlotRepository( XnovaContext context) 
        {
            _context = context;
        }

        public void RemoveRange(IEnumerable<BookingSlot> bookingSlots)
        {
            _context.BookingSlots.RemoveRange(bookingSlots);
        }
        public async Task<IEnumerable<BookingSlot>> GetAllAsync(Expression<Func<BookingSlot, bool>> predicate)
        {
            return await _context.BookingSlots.Where(predicate).ToListAsync();
        }

    }

}
