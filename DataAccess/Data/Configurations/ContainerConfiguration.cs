using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Configurations
{
    public class ContainerConfiguration : IEntityTypeConfiguration<Container>
    {
        public void Configure(EntityTypeBuilder<Container> builder)
        {
            builder.HasMany(x => x.Cards)
                .WithOne(x => x.Container)
                .HasForeignKey(x => x.ContainerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); //  <= this will prevent that error
        }
    }
}
