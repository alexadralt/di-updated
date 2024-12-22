using TagCloud;
using TagCloud.Logger;
using TagCloud.WordPreprocessor;

namespace ConsoleClient;

public class CLIClient(
    IWordCloudImageGenerator wordCloudImageGenerator,
    ILogger logger,
    IWordDelimiterProvider wordDelimiterProvider,
    IBoringWordProvider boringWordProvider)
{
    public void RunOptions(Options options)
    {
        if (options.WordDelimiterFile != null!)
        {
            wordDelimiterProvider.LoadDelimitersFile(options.WordDelimiterFile);
        }

        if (options.BoringWordsFile != null!)
        {
            boringWordProvider.LoadBoringWordsFile(options.BoringWordsFile);
        }

        var imageFileExtension = Path.GetExtension(options.OutputFile);
        if (!wordCloudImageGenerator.IsSupportedImageFileExtension(imageFileExtension))
        {
            logger.Error($"Unsupported output file extension: {imageFileExtension}");
            logger.Error($"Supported extensions are: {string.Join(", ",
                wordCloudImageGenerator.GetSupportedImageFileExtensions())}");
            return;
        }
        
        if (wordCloudImageGenerator.TryGenerateImageFromFile(options.InputFile))
            wordCloudImageGenerator.SaveImageToFile(options.OutputFile);
    }
}