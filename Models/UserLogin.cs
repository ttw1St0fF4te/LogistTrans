using System.ComponentModel.DataAnnotations.Schema;

namespace LogistTrans.Models;

public class UserLogin
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string PassHash { get; set; }

    [ForeignKey("Role")]
    public int RoleId { get; set; }
    public Role Role { get; set; }
}
