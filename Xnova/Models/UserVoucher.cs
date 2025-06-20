using System;
using System.Collections.Generic;

namespace Xnova.Models;

public partial class UserVoucher
{
    public int Id { get; set; }

    public DateTime? ReceiveDate { get; set; }

    public int? UserId { get; set; }

    public int? VoucherId { get; set; }

    public virtual User? User { get; set; }

    public virtual Voucher? Voucher { get; set; }
}
