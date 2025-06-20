using System;
using System.Collections.Generic;

namespace Xnova.Models;

public partial class Voucher
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public int? Amount { get; set; }

    public int? MinEffect { get; set; }

    public int? MaxEffect { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<UserVoucher> UserVouchers { get; set; } = new List<UserVoucher>();
}
