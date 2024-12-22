namespace TagCloud;

public interface IWordCloudImageGenerator
{
    public bool TryGenerateImageFromFile(string filePath);
    public void SaveImageToFile(string filePath);
    
    public bool IsSupportedImageFileExtension(string fileExtension);
    public IEnumerable<string> GetSupportedImageFileExtensions();
}