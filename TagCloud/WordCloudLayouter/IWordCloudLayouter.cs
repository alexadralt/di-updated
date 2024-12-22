using System.Drawing;
using TagCloud.WordStatistics;

namespace TagCloud.WordCloudLayouter;

public interface IWordCloudLayouter
{
    public void DrawWordCloud(Graphics graphics);
    public IWordStatistics WordStatistics { get; }
}