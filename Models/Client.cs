using System.ComponentModel.DataAnnotations.Schema;

namespace LogistTrans.Models;

public class Client
{
    public int Id { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string CompanyName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public DateTime RegistrationDate { get; set; }

    [ForeignKey("Login")]
    public int LoginId { get; set; }
    public UserLogin Login { get; set; }
}
