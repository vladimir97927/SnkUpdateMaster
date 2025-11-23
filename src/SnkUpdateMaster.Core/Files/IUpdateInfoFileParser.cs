namespace SnkUpdateMaster.Core.Files
{
    public interface IUpdateInfoFileParser
    {
        UpdateInfo Parse(string filePath);

        UpdateInfo Parse(byte[] fileBytes);
    }
}
