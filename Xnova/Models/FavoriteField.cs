using System;
using System.Collections.Generic;

namespace Xnova.Models;

public partial class FavoriteField
{
    public int Id { get; set; }

    public DateTime? SetDate { get; set; }

    public int? UserId { get; set; }

    public int? FieldId { get; set; }

    public virtual Field? Field { get; set; }

    public virtual User? User { get; set; }
}
