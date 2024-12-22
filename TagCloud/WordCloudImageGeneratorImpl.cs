using System.Drawing;
using System.Drawing.Imaging;
using TagCloud.FileReader;
using TagCloud.Logger;
using TagCloud.WordPreprocessor;
using TagCloud.WordRenderer;
using TagCloud.WordStatistics;

namespace TagCloud;

public class WordCloudImageGeneratorImpl(
    ILogger logger,
    FileReaderRegistry readerRegistry,
    IWordPreprocessor wordPreprocessor,
    IWordStatistics wordStatistics,
    IWordRenderer wordRenderer
    ): IWordCloudImageGenerator
{
    private Bitmap? _bitmap;
    
    public bool TryGenerateImageFromFile(string filePath)
    {
        if (!Path.Exists(filePath))
        {
            logger.Error($"Could not find input file: {Path.GetFullPath(filePath)}");
            return false;
        }
        
        var extension = Path.GetExtension(filePath);
        if (readerRegistry.TryGetFileReader(extension, out var fileReader))
        {
            fileReader.OpenFile(Path.GetFullPath(filePath));
            while (fileReader.TryGetNextLine(out var line))
            {
                var words = wordPreprocessor.ExtractWords(line);
                wordStatistics.Populate(words);
            }
            readerRegistry.ReturnFileReader(fileReader);
            _bitmap = wordRenderer.Render();
        }
        else
        {
            logger.Error("Input file is not supported.");
            return false;
        }
        return true;
    }

    public void SaveImageToFile(string filePath)
    {
        if (_bitmap == null)
            throw new InvalidOperationException("Image was not generated yet.");
        
        if (Path.GetExtension(filePath) != ".png")
        {
            logger.Error("Only .png files are supported.");
            return;
        }
        _bitmap.Save(filePath, ImageFormat.Png);
        
        logger.Info($"Output file is saved to {Path.GetFullPath(filePath)}");
    }
}