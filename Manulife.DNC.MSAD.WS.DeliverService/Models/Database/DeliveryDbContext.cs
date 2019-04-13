using Microsoft.EntityFrameworkCore;

namespace Manulife.DNC.MSAD.WS.DeliveryService.Models
{
    public class DeliveryDbContext : DbContext
    {
        public DeliveryDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Delivery> Deliveries { get; set; }
    }
}
