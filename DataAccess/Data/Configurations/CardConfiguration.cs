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
    public class CardConfiguration : IEntityTypeConfiguration<Card>
    {
        public void Configure(EntityTypeBuilder<Card> builder)
        {
            builder.HasMany(x => x.ContainerCards)
                .WithOne(x => x.Card)
                .HasForeignKey(x => x.ContainerId)
                .IsRequired();
        }
    }
}
