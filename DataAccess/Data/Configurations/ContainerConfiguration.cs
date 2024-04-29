using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLeit.Entities;

namespace DataAccessLeit.Data.Configurations
{
    public class ContainerConfiguration : IEntityTypeConfiguration<Container>
    {
        public void Configure(EntityTypeBuilder<Container> builder)
        {
            builder.HasMany(x => x.ContainerCards)
                .WithOne(x => x.Container)
                .HasForeignKey(x => x.ContainerId)
                .IsRequired();
                //.OnDelete(DeleteBehavior.Cascade);
        }
    }
}
