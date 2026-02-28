using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DrakthulJelita.Web.Models;

[Index(nameof(Path), IsUnique = true)]
[Index(nameof(WowClassId))]
[Index(nameof(WowClassId), nameof(WowName))]
public class Screenshot
{
    public int Id { get; set; }

    [Required] [StringLength(255)] public string Path { get; set; } = null!;

    [Required] [StringLength(32)] public string MimeType { get; set; } = null!;

    public int Size { get; set; }

    [Required]
    [StringLength(16, MinimumLength = 2)]
    public string WowName { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int WowClassId { get; set; }

    public WowClass WowClass { get; set; } = null!;

    public int Width { get; set; }

    public int Height { get; set; }
}