using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xnova.Models;
using Xnova.Repositories;

namespace Xnova
{
    public class UnitOfWork
    {
        private XnovaContext _context;
        private BookingRepository _bookingRepository;

        public UnitOfWork(XnovaContext context)
        {
            _context = context;
        }
        public BookingRepository BookingRepository
        {
            get { return _bookingRepository ??= new BookingRepository(_context); }
        }
    }
}
