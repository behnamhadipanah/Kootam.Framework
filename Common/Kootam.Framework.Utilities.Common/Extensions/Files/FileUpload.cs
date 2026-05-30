namespace Kootam.Framework.Utilities.Extensions.Files
{
    public record FileUpload
    {
        public string DirectoryName { get; set; }
        public string Path { get; set; }
        public string UniqueName { get; set; }
    }
}
