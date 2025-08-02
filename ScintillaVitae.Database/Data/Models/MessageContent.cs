using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScintillaVitae.Database.Data.Models
{
    [Table("MessageContents")]
    public class MessageContent
    {
        [Key]
        public long Id { get; set; } //PK

        [Required]
        required public ulong MessageId { get; set; }

        [Required]
        required public MessageRole Role { get; set; }

        [Required]
        required public string Content { get; set; }

        [Required]
        required public DateTime Timestamp { get; set; }

        //FK
        [Required]
        public long InteractionId { get; set; }

        [ForeignKey("InteractionId")]
        public virtual Interaction? Interaction { get; set; }
    }
}