using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Models
{
    public class Message : IEntityTypeConfiguration<Message>
    {
        public long Id { get; set; }

        public string Content { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).ValueGeneratedOnAdd();

            builder.Property(m => m.Content).HasMaxLength(3096);
        }
    }
}