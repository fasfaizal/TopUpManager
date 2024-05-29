using Microsoft.EntityFrameworkCore;
using TopUpManager.Common.Entity;

namespace TopUpManager.DataAccess.DBContext
{
    public class TopUpManagerDbContext : DbContext
    {
        public TopUpManagerDbContext(DbContextOptions<TopUpManagerDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Beneficiary> Beneficiaries { get; set; }
        public DbSet<TopUpTransaction> TopUpTransactions { get; set; }
    }
}
