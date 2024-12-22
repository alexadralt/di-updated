using System.Drawing;
using TagCloud.FileReader;
using TagCloud.ImageFileWriter;
using TagCloud.Logger;
using TagCloud.WordPreprocessor;
using TagCloud.WordRenderer;

namespace TagCloud;

public class WordCloudImageGeneratorImpl(
    ILogger logger,
    FileReaderRegistry readerRegistry,
    ImageFileWriterRegistry imageWriterRegistry,
    IWordPreprocessor wordPreprocessor,
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
                wordRenderer.WordStatistics.Populate(words);
            }
            readerRegistry.ReturnFileReader(fileReader);
            _bitmap = wordRenderer.Render();
        }
        else
        {
            logger.Error($"Unsupported input file extension: {extension}");
            logger.Error($"Supported extensions are: {string.Join(", ",
                readerRegistry.GetSupportedFileExtensions())}");
            return false;
        }
        return true;
    }

    public void SaveImageToFile(string filePath)
    {
        if (_bitmap == null)
            throw new InvalidOperationException("Image was not generated yet.");
        
        if (imageWriterRegistry.TryGetImageFileWriter(Path.GetExtension(filePath), out var imageWriter))
        {
            imageWriter.SaveImage(_bitmap, filePath);
        }
        else
        {
            throw new ArgumentException($"Unsupported image format: {Path.GetExtension(filePath)}");
        }
        
        logger.Info($"Output file is saved to {Path.GetFullPath(filePath)}");
    }

    public bool IsSupportedImageFileExtension(string fileExtension)
    {
        return imageWriterRegistry.IsSupportedExtension(fileExtension);
    }

    public IEnumerable<string> GetSupportedImageFileExtensions()
    {
        return imageWriterRegistry.GetSupportedImageFileExtensions();
    }
}