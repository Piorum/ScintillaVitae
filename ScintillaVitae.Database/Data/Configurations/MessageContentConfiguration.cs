using ScintillaVitae.Database.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScintillaVitae.Database.Data.Configurations;

public class MessageContentConfiguration : IEntityTypeConfiguration<MessageContent>
{
    public void Configure(EntityTypeBuilder<MessageContent> builder)
    {
        builder.HasIndex(m => m.Timestamp);
    }
}
