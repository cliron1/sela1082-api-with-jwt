using System.ComponentModel.DataAnnotations;

namespace MyApi.Data;

public class Account {
    public int Id { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    public string Pwd { get; set; }
}
