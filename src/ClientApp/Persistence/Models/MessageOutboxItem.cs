using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Models
{
    public class MessageOutboxItem : IEntityTypeConfiguration<MessageOutboxItem>
    {
        public long MessageId { get; set; }

        public DateTimeOffset? SentAt { get; set; }

        public virtual Message Message { get; set; }

        public void Configure(EntityTypeBuilder<MessageOutboxItem> builder)
        {
            builder.HasKey(mo => mo.MessageId);

            builder.HasOne(mo => mo.Message).WithOne().HasForeignKey<MessageOutboxItem>(mo => mo.MessageId);

            builder.HasIndex(mo => mo.SentAt);
        }
    }
}