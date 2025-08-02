using ScintillaVitae.Database.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScintillaVitae.Database.Data.Configurations;

public class InteractionConfiguration : IEntityTypeConfiguration<Interaction>
{
    public void Configure(EntityTypeBuilder<Interaction> builder)
    {
        builder.HasIndex(i => new { i.ServerId, i.ThreadId })
            .IsUnique();

        builder.HasMany(i => i.MessageHistory)
            .WithOne(m => m.Interaction)
            .HasForeignKey(m => m.InteractionId)
            .IsRequired();
    }
}
