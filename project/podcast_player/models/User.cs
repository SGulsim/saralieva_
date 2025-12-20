using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Неверный формат email")]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    public string PasswordHash { get; set; } = string.Empty;

    [Required(ErrorMessage = "Имя обязательно")]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? LastName { get; set; }

    [Column("DefaultPlaybackSpeed")]
    public double DefaultPlaybackSpeed { get; set; } = 1.0;

    [Required]
    [Column("Role")]
    public Role Role { get; set; } = Role.User;
}
