using Microsoft.EntityFrameworkCore;

namespace MyApi.Data;

public class MyContext : DbContext {
    public MyContext(DbContextOptions options)
        : base(options) {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Product> Products { get; set; }
}
