namespace Api.Responses;

public class OpenAccountResponse
{
    public int CustomerId { get; set; }
    public int AccountId { get; set; }
    public bool Succeeded { get; set; }
}