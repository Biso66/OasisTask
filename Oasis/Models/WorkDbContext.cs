using Microsoft.EntityFrameworkCore;

namespace Oasis.Models
{
    public class WorkDbContext: DbContext
    {
        public WorkDbContext(DbContextOptions options): base(options)
        {
            
        }
        public WorkDbContext()
        {
            
        }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Work> Works { get; set; }
    }
}
