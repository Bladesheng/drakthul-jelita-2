namespace DrakthulJelita.Web.ViewModels;

public sealed class ScreenshotIndexVm
{
    public required IReadOnlyList<WowClassVm> WowClasses { get; init; }
    public required IReadOnlyDictionary<int, IReadOnlyList<ScreenshotVm>> Screenshots { get; init; }

    public sealed class WowClassVm
    {
        public int Id { get; init; }
        public required string Name { get; init; }
        public required string Color { get; init; }
    }

    public sealed class ScreenshotVm
    {
        public int Id { get; init; }
        public required string Path { get; init; }
        public required string WowName { get; init; }
        public int WowClassId { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
        public required string WowClassName { get; init; }
        public required string WowClassColor { get; init; }
    }
}