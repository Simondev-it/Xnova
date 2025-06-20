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
    public class VoucherRepository : GenericRepository<Voucher>
    {
        public VoucherRepository(XnovaContext context) => _context = context;

        public async Task<List<Voucher>> GetAllAsync()
        {
            return await _context.Vouchers.Include(p => p.UserVouchers).ToListAsync();
        }

        public async Task<Voucher> GetByIdAsync(int id)
        {
            var result = await _context.Vouchers.Include(p => p.UserVouchers).FirstAsync(p => p.Id == id);

            return result;

        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
