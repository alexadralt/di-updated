using System.Diagnostics;
using System.Drawing;
using TagCloud.FileHandler;
using TagCloud.Logger;
using TagCloud.WordPreprocessor;
using TagCloud.WordRenderer;

namespace TagCloud;

public class WordCloudImageGeneratorImpl(
    ILogger logger,
    IFileHandler fileHandler,
    IWordPreprocessor wordPreprocessor,
    IWordRenderer wordRenderer
    ): IWordCloudImageGenerator
{
    private Bitmap? _bitmap;
    
    public bool TryGenerateImageFromFile(string filePath)
    {
        if (!fileHandler.IsValidInputFile(filePath, out var error))
        {
            logger.Error(error!);
            return false;
        }

        foreach (var line in fileHandler.ReadAllLines(filePath))
        {
            var words = wordPreprocessor.ExtractWords(line);
            wordRenderer.WordStatistics.Populate(words);
        }

        try
        {
            _bitmap = wordRenderer.Render();
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message);
            return false;
        }
        return true;
    }

    public void SaveImageToFile(string filePath)
    {
        if (_bitmap == null)
            throw new InvalidOperationException("Image was not generated yet.");
        
        fileHandler.SaveImage(_bitmap, filePath);
    }

    public bool IsSupportedOutputFileExtension(string? filePath, out string? errorMessage)
    {
        var extension = Path.GetExtension(filePath);
        if (string.IsNullOrEmpty(extension))
        {
            errorMessage = $"Missing output file extension: {filePath} <---";
            return false;
        }

        var isSupported = fileHandler.IsSupportedOutputFileExtension(extension);
        errorMessage = isSupported
            ? null
            : $"Unsupported output file extension: {extension}\n"
              + $"Supported extensions are: {string.Join(", ",
                  fileHandler.GetSupportedOutputFileExtensions())}";
        return isSupported;
    }

    public bool DoesOutputFileExist(string filePath)
    {
        return Path.Exists(filePath);
    }

    public void LoadWordDelimitersFile(string filePath)
    {
        wordPreprocessor.LoadWordDelimitersFile(filePath);
    }

    public void LoadBoringWordsFile(string filePath)
    {
        wordPreprocessor.LoadBoringWordsFile(filePath);
    }
}