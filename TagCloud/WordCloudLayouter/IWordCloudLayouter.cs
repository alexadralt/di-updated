using System.Drawing;

namespace TagCloud.WordCloudLayouter;

public interface IWordCloudLayouter
{
    public void DrawWordCloud(Graphics graphics);
}