using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScintillaVitae.Database.Data.Models
{
    [Table("Interactions")]
    public class Interaction
    {
        [Key]
        public long Id { get; set; } //PK

        [Required]
        required public ulong ServerId { get; set; }

        [Required]
        required public ulong ThreadId { get; set; }

        // Navigation Property:
        // This tells EF Core that one Interaction can have a collection of many MessageContents.
        public virtual ICollection<MessageContent> MessageHistory { get; set; } = [];
    }
}