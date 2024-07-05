using System.Data.Common;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Test.Helpers;

namespace Test.Fixtures;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly string ConnectionString;
    private readonly AccountDbContext Context;

    public CustomWebApplicationFactory()
    {
        UserSecretsFixture userSecretsFixture = new();
        ConnectionString = userSecretsFixture.ConnectionString;
        var options = new DbContextOptionsBuilder<AccountDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;
        Context = new AccountDbContext(options);

        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // remove any existing DbContext services from web application factory
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<AccountDbContext>));

            services.Remove(dbContextDescriptor!);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbConnection));

            services.Remove(dbConnectionDescriptor!);

            services.AddDbContext<AccountDbContext>((container, options) =>
                options.UseSqlServer(ConnectionString));
        });

        builder.UseEnvironment("Development");
    }

    public async Task CleanupAsync()
    {
        await DataUtilities.ReinitializeDbForTestsAsync(Context);
    }
}