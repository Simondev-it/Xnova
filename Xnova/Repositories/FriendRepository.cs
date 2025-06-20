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
    public class FriendRepository : GenericRepository<Friend>
    {
        public FriendRepository(XnovaContext context) => _context = context;
    }
}
