using CommandLine;
using ConsoleClient;
using Pure.DI;
using TagCloud.FileReader;
using TagCloud.Logger;
using TagCloud.SettingsProvider;
using TagCloud.TagsCloudVisualization;
using TagCloud.WordCloudLayouter;
using TagCloud.WordPreprocessor;
using TagCloud.WordRenderer;
using TagCloud.WordStatistics;

DI.Setup("Composition")
    .Bind<IFileReader>().To<TxtFileReader>()
    .Bind<IWordPreprocessor>().To<TagPreprocessor>()
    .Bind<IWordRenderer>().To<TagCloudWordRenderer>()
    .Bind<ICircularCloudLayouter>().To<CircularCloudLayouterImpl>()
    .Hint(Hint.Resolve, "Off")
    .Bind().As(Lifetime.Singleton).To<WordStatisticsImpl>()
    .Bind().As(Lifetime.Singleton).To<WordCloudLayouterImpl>()
    .Bind().As(Lifetime.Singleton).To<BoringWordProviderImpl>()
    .Bind().As(Lifetime.Singleton).To<WordDelimiterProviderImpl>()
    .Bind().As(Lifetime.Singleton).To<ConsoleLogger>()
    .Bind().As(Lifetime.Singleton).To<SettingsProviderImpl>()
    .Bind().As(Lifetime.Singleton).To<CLIClient>()
    .Root<CLIClient>("Client");

using var composition = new Composition();
var client = composition.Client;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(client.RunOptions);