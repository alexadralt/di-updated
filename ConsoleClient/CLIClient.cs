using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using CommandLine;
using TagCloud.FileReader;
using TagCloud.Logger;
using TagCloud.WordPreprocessor;
using TagCloud.WordRenderer;
using TagCloud.WordStatistics;

namespace ConsoleClient;

public class CLIClient(
    FileReaderRegistry readerRegistry,
    IWordPreprocessor wordPreprocessor,
    IWordStatistics wordStatistics,
    IWordRenderer wordRenderer,
    ILogger logger,
    IWordDelimiterProvider wordDelimiterProvider,
    IBoringWordProvider boringWordProvider)
    : IDisposable
{
    private FileReaderRegistry _readerRegistry = readerRegistry;
    private bool _isDisposed;

    public void RunOptions(Options options)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (options.WordDelimiterFile != null!)
        {
            wordDelimiterProvider.LoadDelimitersFile(options.WordDelimiterFile);
        }

        if (options.BoringWordsFile != null!)
        {
            boringWordProvider.LoadBoringWordsFile(options.BoringWordsFile);
        }
        
        Bitmap bitmap = null;
        if (options.InputFile != null!)
        {
            if (Path.Exists(options.InputFile))
            {
                var extension = Path.GetExtension(options.InputFile);
                if (_readerRegistry.TryGetFileReader(extension, out var fileReader))
                {
                    fileReader.OpenFile(Path.GetFullPath(options.InputFile));
                    var sb = new StringBuilder();
                    while (fileReader.TryGetNextLine(out var line))
                        sb.AppendLine(line);
                    fileReader.Dispose();

                    var words = wordPreprocessor.ExtractWords(sb.ToString());
                    wordStatistics.Populate(words);
                    bitmap = wordRenderer.Render();
                }
                else
                {
                    logger.Error("Input file is not supported.");
                }
            }
            else
            {
                logger.Error($"Could not find input file: {Path.GetFullPath(options.InputFile)}");
            }
        }

        if (options.OutputFile != null!)
        {
            if (bitmap != null)
            {
                bitmap.Save(options.OutputFile, ImageFormat.Png);
                logger.Info($"Output file is saved to {Path.GetFullPath(options.OutputFile)}");
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
                _readerRegistry.Dispose();
            _readerRegistry = null;
            _isDisposed = true;
        }
    }
}