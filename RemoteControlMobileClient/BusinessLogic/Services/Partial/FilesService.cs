namespace RemoteControlMobileClient.BusinessLogic.Services.Partial
{
    internal partial class FilesService
    {
        public partial Task<bool> SaveFileAsync(byte[] fileBytes, string fileName);
        public partial Task<bool> SaveFileAsync(string fileName, Stream stream, IProgress<double> progress = null, CancellationToken token = default);
        public partial string CreateUniqueDownloadPath(string fileName);

	}
}
