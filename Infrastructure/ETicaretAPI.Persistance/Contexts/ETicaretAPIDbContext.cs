using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Common;
using ETicaretAPI.Domain.Entities.Files;
using File = ETicaretAPI.Domain.Entities.Files.File;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ETicaretAPI.Domain.Entities.Identity;

namespace ETicaretAPI.Persistance.Contexts
{
    public class ETicaretAPIDbContext : IdentityDbContext<AppUser,AppRole, string>
    {
        public ETicaretAPIDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product> Products{ get; set; }
        public DbSet<Customer> Customers{ get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<ProductImageFile> ProductImageFiles { get; set; }
        

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var datas = ChangeTracker.Entries<BaseEntity>();
            foreach(var data in datas)
            {
                if (data.State == EntityState.Added)
                {
                    data.Entity.CreatedDate = DateTime.UtcNow;
                }
                else if (data.State == EntityState.Modified)
                {
                    data.Entity.UpdatedDate = DateTime.UtcNow;
                    data.Property(x => x.CreatedDate).IsModified = false;
                }
            }
                
            return await base.SaveChangesAsync(cancellationToken);
        }

    }
}
