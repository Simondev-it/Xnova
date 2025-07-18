﻿using Microsoft.EntityFrameworkCore;
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
    public  class PaymentRepository : GenericRepository<Payment>
    {
        public PaymentRepository(XnovaContext context) => _context = context;

        //public async Task<List<Payment>> GetAllAsync()
        //{
        //    return await _context.Payments.Include(p => p.Products).ToListAsync();
        //}

        //public async Task<Category> GetByIdAsync(int id)
        //{
        //    var result = await _context.Categories.Include(p => p.Products).FirstAsync(p => p.Id == id);

        //    return result;
        //}

        public async Task<Payment> GetFirstOrDefaultAsync(Expression<Func<Payment, bool>> predicate)
        {
            return await _context.Set<Payment>().FirstOrDefaultAsync(predicate);
        }
        public async Task<Payment?> GetByBookingIdAsync(int bookingId)
        {
            return await _context.Payments
                .Where(p => p.BookingId == bookingId && p.Status == 0) // ⚠️ chỉ lấy bản chưa thanh toán
                .OrderByDescending(p => p.Id) // Ưu tiên bản mới nhất
                .FirstOrDefaultAsync();
        }
        public async Task<Payment> GetByOrderIdAsync(int orderId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.BookingId == orderId); // Giả sử BookingId là OrderId
        }
        public async Task<Payment> GetAsync(Expression<Func<Payment, bool>> predicate)
        {
            return await _context.Payments.FirstOrDefaultAsync(predicate);
        }
        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }
        public void RemoveRange(IEnumerable<Payment> payments)
        {
            _context.Payments.RemoveRange(payments);
        }
        public async Task<IEnumerable<Payment>> GetAllAsync(Expression<Func<Payment, bool>> predicate)
        {
            return await _context.Payments.Where(predicate).ToListAsync();
        }
        public async Task UpdateAsync1(Payment payment)
        {
            _context.Attach(payment); // đảm bảo EF đang tracking entity này

            // Chỉ định rõ các property cần cập nhật
            _context.Entry(payment).Property(p => p.Status).IsModified = true;
            _context.Entry(payment).Property(p => p.Response).IsModified = true;
            _context.Entry(payment).Property(p => p.Date).IsModified = true;

            await _context.SaveChangesAsync();
        }



    }
}
