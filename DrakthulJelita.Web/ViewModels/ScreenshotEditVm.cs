using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DrakthulJelita.Web.ViewModels;

public sealed class ScreenshotEditVm
{
    public required InputVm Input { get; init; }

    [ValidateNever] public required IReadOnlyList<WowClassVm> WowClasses { get; set; }
    [ValidateNever] public required DisplayVm Display { get; set; }

    public sealed class InputVm
    {
        [Required]
        [StringLength(16, MinimumLength = 2)]
        public string WowName
        {
            get;
            init => field = value.Replace(" ", "").ToLower();
        } = "";

        [Required] [Range(1, 13)] public int WowClassId { get; init; }
    }

    public sealed class DisplayVm
    {
        public required string Path { get; init; }
        public required int Width { get; init; }
        public required int Height { get; init; }
    }
}