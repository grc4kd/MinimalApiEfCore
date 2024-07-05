namespace Test.Fixtures;

public class UserSecretsFixture
{
    internal readonly string ConnectionString;

    public UserSecretsFixture()
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets<UserSecretsFixture>()
            .Build();

        ConnectionString = config["ConnectionStrings:AccountsTestDatabaseConnection"] ?? "";
    }
}