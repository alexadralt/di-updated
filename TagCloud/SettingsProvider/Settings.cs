using System.Drawing;

namespace TagCloud.SettingsProvider;

public record Settings
{
    public Color TextColor { get; init; }
    public Color BackgroundColor { get; init; }
    public FontFamily Font { get; init; }
    public int MinFontSize { get; init; }
    public int MaxFontSize { get; init; }
    public Size ImageSize { get; init; }
}