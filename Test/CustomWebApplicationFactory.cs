using System.Data.Common;
using Api.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Test;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
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

            // setup an open SqliteConnection so EF won't automatically close it
            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqliteConnection("Datasource=:memory:");
                connection.Open();

                return connection;
            });

            services.AddDbContext<AccountDbContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });
        });

        builder.UseEnvironment("Development");
    }
}