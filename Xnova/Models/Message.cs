using System;
using System.Collections.Generic;

namespace Xnova.Models;

public partial class Message
{
    public int Id { get; set; }

    public string? Content { get; set; }

    public DateTime? Date { get; set; }

    public int? Status { get; set; }

    public int? ChatboxId { get; set; }

    public virtual Chatbox? Chatbox { get; set; }
}
