using Games.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests
{
    public class IntegrationTestsWebApplicationFactory<TProgram> :
        WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DataContext>));
#pragma warning disable CS8604 // Possible null reference argument.
                services.Remove(dbContextDescriptor);
#pragma warning restore CS8604 // Possible null reference argument.

                var dbConnectionDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbConnection));
#pragma warning disable CS8604 // Possible null reference argument.
                services.Remove(dbConnectionDescriptor);
#pragma warning restore CS8604 // Possible null reference argument.

                // Create open SqliteConnection so EF won't automatically close it.
                services.AddSingleton<DbConnection>(container =>
                {
                    var connection = new SqliteConnection("DataSource=:memory:");
                    connection.Open();

                    return connection;
                });

                services.AddDbContext<DataContext>((container, options) =>
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    var opt = options.UseSqlite(connection).Options;
                });
            });

            builder.UseEnvironment("Development");
        }
    }
}
