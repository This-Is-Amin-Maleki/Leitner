using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLeit.Entities;
using System.Reflection.Emit;

namespace DataAccessLeit.Data.Configurations
{
    public class ContainerCardConfiguration : IEntityTypeConfiguration<ContainerCard>
    {
        public void Configure(EntityTypeBuilder<ContainerCard> builder)
        {
            builder.HasKey(x => x.Id);// new {x.ContainerId,x.CardId});
            builder.HasOne(x => x.Container)
                .WithMany(x => x.ContainerCards)
                .HasForeignKey(x => x.ContainerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); //  <= this will prevent that error

            builder.HasOne(x => x.Card)
                .WithMany(x => x.ContainerCards)
                .HasForeignKey(x => x.CardId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); //  <= this will prevent that error
        }
    }
}
