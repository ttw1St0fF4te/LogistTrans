namespace LogistTrans.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

public class Order
{
    public int Id { get; set; }

    [ForeignKey("Client")]
    public int ClientId { get; set; }
    public Client Client { get; set; }

    public DateTime OrderDate { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string Status { get; set; } // Заказано/В пути/Доставлено

    public List<OrderItem> OrderItems { get; set; }

    public Order()
    {
        OrderItems = new List<OrderItem>();
    }
}
