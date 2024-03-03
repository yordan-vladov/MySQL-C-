using Microsoft.EntityFrameworkCore;

public class ProductContext : DbContext
{
    public ProductContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql("server=localhost;userid=yordan;password=1234;database=shop;",
        new MySqlServerVersion(new Version(10, 5, 23)));
    }
    public DbSet<Product> Products { get; set; }
}