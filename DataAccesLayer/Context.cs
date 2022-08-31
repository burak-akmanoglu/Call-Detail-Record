using EntityLayer;
using Microsoft.EntityFrameworkCore;

namespace DataAccesLayer
{
    public class Context:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=BURAK;database=Innova;integrated security=true");
        }
        public DbSet<User> TblUser { get; set; }
        public DbSet<ClientData> TblClientData { get; set; }
    }
}
