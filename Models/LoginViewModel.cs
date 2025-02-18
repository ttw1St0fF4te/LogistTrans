namespace LogistTrans.Models;

using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    // Свойство для логина пользователя
    [Required(ErrorMessage = "Логин обязателен.")]
    [StringLength(50, ErrorMessage = "Логин не должен превышать 50 символов.")]
    [Display(Name = "Логин")]
    public string Login { get; set; }

    // Свойство для пароля пользователя
    [Required(ErrorMessage = "Пароль обязателен.")]
    [StringLength(50, ErrorMessage = "Пароль не должен превышать 50 символов.", MinimumLength = 6)]
    [DataType(DataType.Password)] // Указывает, что это поле для пароля (скрытый ввод)
    [Display(Name = "Пароль")]
    public string Password { get; set; }
}