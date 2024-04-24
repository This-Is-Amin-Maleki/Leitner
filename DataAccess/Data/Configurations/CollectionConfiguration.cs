using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;

namespace DataAccess.Data.Configurations
{
    public class CollectionConfiguration : IEntityTypeConfiguration<Collection>
    {
        public void Configure(EntityTypeBuilder<Collection> builder)
        {
            builder.HasMany(x=>x.Boxes)
                .WithOne(x => x.Collection)
                .HasForeignKey(x => x.CollectionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); //  <= this will prevent that error
        }
    }
}
