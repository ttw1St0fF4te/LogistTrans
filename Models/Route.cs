using System.ComponentModel.DataAnnotations.Schema;

namespace LogistTrans.Models;

public class Route
{
    public int Id { get; set; }

    public double Distance { get; set; }

    public DateTime DepartureTime { get; set; }

    public TimeSpan TravelTime { get; set; }

    public DateTime ArrivalTime { get; set; }

    [ForeignKey("Order")]
    public int OrderId { get; set; }
    public Order Order { get; set; }

    [ForeignKey("Employee")]
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
}