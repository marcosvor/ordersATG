using OrderAccumulatorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace OrderAccumulatorApp.DB;

public class OrderAccumulatorDbContext : DbContext
{

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false);
        IConfiguration config = builder.Build();
        var dbConnection = config.GetSection("DbConnection").Get<OrderAccumulatorDbConfig>();
        string dbConnectionString = $"Host={dbConnection.Host};Port={dbConnection.Port};Database={dbConnection.Database};User Id={dbConnection.UserId};Password={dbConnection.Password};";
        optionsBuilder.UseNpgsql(dbConnectionString);
    }

    public DbSet<Orders> Orders { get; set; }
}