using System.ComponentModel.DataAnnotations;

namespace DrakthulJelita.Web.Models;

public class WowClass
{
    public int Id { get; set; }

    [Required] [StringLength(100)] public string Name { get; set; } = null!;

    [Required] [StringLength(16)] public string Color { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Screenshot> Screenshots { get; set; } = new List<Screenshot>();
}