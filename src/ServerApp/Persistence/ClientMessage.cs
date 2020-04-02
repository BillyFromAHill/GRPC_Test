using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence
{
    public class ClientMessage : IEntityTypeConfiguration<ClientMessage>
    {
        public string ClientId { get; set; }

        public string IpAddress { get; set; }

        public long MessageId { get; set; }

        public string Content { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset ReceivedAt { get; set; }

        public void Configure(EntityTypeBuilder<ClientMessage> builder)
        {
            builder.HasKey(m => new {m.MessageId, m.ClientId});

            builder.Property(m => m.Content).HasMaxLength(3096);

            builder.Property(m => m.Content).HasMaxLength(3096);

            builder.Property(m => m.IpAddress).HasMaxLength(15);

            builder.HasIndex(cm => cm.ReceivedAt);
        }
    }
}