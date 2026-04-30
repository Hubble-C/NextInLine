using System.ComponentModel.DataAnnotations.Schema;
using NextInLine.Enums;

namespace NextInLine.Models;

public class Ticket
{
    public int Id { get; set; }
    
    [Column("user_id")]
    public int UserId { get; set; }

    public string Code { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    public TicketStatus Status { get; set; }
}