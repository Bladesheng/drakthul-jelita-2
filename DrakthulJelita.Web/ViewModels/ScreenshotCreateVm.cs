using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DrakthulJelita.Web.ViewModels;

public sealed class ScreenshotCreateVm
{
    public InputVm Input { get; init; } = null!;

    [ValidateNever] public required IReadOnlyList<WowClassVm> WowClasses { get; set; }
}

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

    [Required] public IFormFile FileUpload { get; init; } = null!;
}