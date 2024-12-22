using System.Drawing;
using System.Drawing.Drawing2D;
using TagCloud.SettingsProvider;
using TagCloud.WordCloudLayouter;
using TagCloud.WordStatistics;

namespace TagCloud.WordRenderer;

public class TagCloudWordRenderer(
    IWordCloudLayouter wordCloudLayouter,
    ISettingsProvider settingsProvider
    ) : IWordRenderer
{
#pragma warning disable CA1416
    public Bitmap Render()
    {
        var settings = settingsProvider.GetSettings();
        
        var imageSize = settings.ImageSize;
        var bitmap = new Bitmap(imageSize.Width, imageSize.Height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(settings.BackgroundColor);
        
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        wordCloudLayouter.DrawWordCloud(graphics);
        return bitmap;
    }
#pragma warning restore CA1416

    public IWordStatistics WordStatistics => wordCloudLayouter.WordStatistics;
}