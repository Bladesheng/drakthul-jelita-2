namespace DrakthulJelita.Web.ViewModels;

public sealed class ScreenshotIndexVm
{
    public required IReadOnlyList<WowClassVm> WowClasses { get; init; }
    public required IReadOnlyDictionary<int, IReadOnlyList<ScreenshotVm>> Screenshots { get; init; }

    public sealed class ScreenshotVm
    {
        public required int Id { get; init; }
        public required string Path { get; init; }
        public required string WowName { get; init; }
        public required int WowClassId { get; init; }
        public required int Width { get; init; }
        public required int Height { get; init; }
        public required string WowClassName { get; init; }
        public required string WowClassColor { get; init; }
    }
}