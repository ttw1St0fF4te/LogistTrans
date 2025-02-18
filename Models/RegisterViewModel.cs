namespace LogistTrans.Models;

using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    // Логин
    [Required(ErrorMessage = "Логин обязателен.")]
    [StringLength(50, ErrorMessage = "Логин не должен превышать 50 символов.")]
    [Display(Name = "Логин")]
    public string Login { get; set; }

    // Пароль
    [Required(ErrorMessage = "Пароль обязателен.")]
    [StringLength(50, ErrorMessage = "Пароль не должен превышать 50 символов.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; }

    // Имя
    [Required(ErrorMessage = "Имя обязательно.")]
    [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов.")]
    [Display(Name = "Имя")]
    public string FirstName { get; set; }

    // Фамилия
    [Required(ErrorMessage = "Фамилия обязательна.")]
    [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов.")]
    [Display(Name = "Фамилия")]
    public string LastName { get; set; }

    // Отчество
    [StringLength(50, ErrorMessage = "Отчество не должно превышать 50 символов.")]
    [Display(Name = "Отчество")]
    public string MiddleName { get; set; }

    // Название компании
    [StringLength(100, ErrorMessage = "Название компании не должно превышать 100 символов.")]
    [Display(Name = "Название компании")]
    public string CompanyName { get; set; }

    // Телефон
    [Phone(ErrorMessage = "Введите корректный телефон.")]
    [Display(Name = "Телефон")]
    public string Phone { get; set; }

    // Email
    [Required(ErrorMessage = "Email обязателен.")]
    [EmailAddress(ErrorMessage = "Введите корректный email.")]
    [Display(Name = "Email")]
    public string Email { get; set; }
}