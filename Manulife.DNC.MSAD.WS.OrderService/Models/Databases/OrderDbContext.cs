using Microsoft.EntityFrameworkCore;

namespace Manulife.DNC.MSAD.WS.OrderService.Models
{
    public class OrderDbContext : DbContext
    {
        public string ConnStr { get; }

        public OrderDbContext(DbContextOptions options, string connStr) : base(options)
        {
            ConnStr = connStr;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnStr);
        }

        public DbSet<Order> Orders { get; set; }
    }
}
