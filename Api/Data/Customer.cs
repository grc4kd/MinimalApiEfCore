using System.ComponentModel.DataAnnotations;

namespace Api.Data;

public class Customer
{
    public int Id { get; set; }

    [DataType(DataType.Text)]
    [StringLength(100, ErrorMessage = "There's a 100 character limit on customer name. Please shorten the name.")]
    public required string Name { get; set; }

    public ICollection<Account> Accounts { get; } = [];
}