namespace TagCloud;

public interface IWordCloudImageGenerator
{
    public bool TryGenerateImageFromFile(string filePath);
    public void SaveImageToFile(string filePath);
    
    public bool IsSupportedImageFileExtension(string fileExtension);
    public IEnumerable<string> GetSupportedImageFileExtensions();
    public void LoadWordDelimitersFile(string filePath);
    public void LoadBoringWordsFile(string filePath);
}