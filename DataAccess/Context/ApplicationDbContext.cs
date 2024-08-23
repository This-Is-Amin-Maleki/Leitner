using DataAccessLeit.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using ModelsLeit.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DataAccessLeit.Context
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser,IdentityRole<long>,long>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options){}
        
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Box> Boxes { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<Container> Containers { get; set; }
        public DbSet<ContainerCard> ContainerCards { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(
                "Database = Leitner;Data Source=ARTEMISIA;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False"
                 );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CollectionConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CardConfiguration).Assembly);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BoxConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContainerConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContainerCardConfiguration).Assembly);
        }
    }
}