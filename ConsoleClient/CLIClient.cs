using TagCloud;
using TagCloud.WordPreprocessor;

namespace ConsoleClient;

public class CLIClient(
    IWordCloudImageGenerator wordCloudImageGenerator,
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

        if (wordCloudImageGenerator.TryGenerateImageFromFile(options.InputFile))
            wordCloudImageGenerator.SaveImageToFile(options.OutputFile);
    }
}