namespace Api;

public sealed class Settings
{
    public required decimal MaxDepositAmount { get; set; }
    public required decimal MaxWithdrawalAmount { get; set; }
}