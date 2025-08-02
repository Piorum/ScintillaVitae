using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ScintillaVitae.Database.Data.Models;

namespace ScintillaVitae.Database.Data;

public class ChatContext(DbContextOptions<ChatContext> options) : DbContext(options)
{
    public DbSet<Interaction> Interactions { get; set; }
    public DbSet<MessageContent> MessageContents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
