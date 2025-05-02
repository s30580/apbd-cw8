using System.ComponentModel.DataAnnotations;

namespace Tutorial8.Models.DTOs;

public class ClientPOST
{
    [Required]
    [MaxLength(120)]
    public string FirstName { get; set; }
    [Required]
    [MaxLength(120)]
    public string LastName { get; set; }
    [Required]
    [MaxLength(120)]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [MaxLength(120)]
    [Phone]
    [RegularExpression(@"^\+\d{11}$",ErrorMessage = "Invalid phone number")]
    public string Telephone { get; set; }
    [Required]
    [MaxLength(120)]
    [RegularExpression(@"^\d{11}$",ErrorMessage = "Pesel must be 11 digit number")]
    public string Pesel { get; set; }
}