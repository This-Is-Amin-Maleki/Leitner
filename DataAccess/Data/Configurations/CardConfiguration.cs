﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Configurations
{
    public class CardConfiguration : IEntityTypeConfiguration<Card>
    {
        public void Configure(EntityTypeBuilder<Card> builder)
        {
            builder.HasOne(x => x.Container)
                .WithMany(x => x.Cards)
                .HasForeignKey(x => x.ContainerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); //  <= this will prevent that error
        }
    }
}
