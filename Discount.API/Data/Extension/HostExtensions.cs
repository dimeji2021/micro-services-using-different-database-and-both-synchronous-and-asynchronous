using Npgsql;

namespace Discount.API.Data.Extension
{
    public class HostExtensions
    {
        public static void MigrateDatabase(IApplicationBuilder builder)
        {
            using var serviceScope = builder.ApplicationServices.CreateScope();
            var _logger = serviceScope.ServiceProvider.GetService<ILogger<HostExtensions>>();
            var _configuration = serviceScope.ServiceProvider.GetService<IConfiguration>();

            int retryForAvailability = 0;
            try
            {
                var x = _configuration.GetValue<string>("DatabaseSettings:ConnectionString");
                _logger.LogInformation("Migrating postresql database.");
                using var connection = new NpgsqlConnection
                    (_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                connection.Open();

                using var command = new NpgsqlCommand
                {
                    Connection = connection
                };

                command.CommandText = "DROP TABLE IF EXISTS Coupon";
                command.ExecuteNonQuery();

                command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY, 
                                                                ProductName VARCHAR(24) NOT NULL,
                                                                Description TEXT,
                                                                Amount INT)";
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('IPhone X', 'IPhone Discount', 150);";
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Samsung 10', 'Samsung Discount', 100);";
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Android', 'Android Discount', 90);";
                command.ExecuteNonQuery();

                _logger.LogInformation("Migrated postresql database.");
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "An error occurred while migrating the postresql database");

                if (retryForAvailability < 50)
                {
                    retryForAvailability++;
                    System.Threading.Thread.Sleep(2000);
                    MigrateDatabase(builder);
                }
            }
        }
    }
}
