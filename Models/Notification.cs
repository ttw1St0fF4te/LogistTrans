using System.ComponentModel.DataAnnotations.Schema;

namespace LogistTrans.Models;

public class Notification
{
    public int Id { get; set; }

    public string Message { get; set; }

    public DateTime SentDate { get; set; }

    [ForeignKey("Client")]
    public int ClientId { get; set; }
    public Client Client { get; set; }

    [ForeignKey("Order")]
    public int OrderId { get; set; }
    public Order Order { get; set; }
}