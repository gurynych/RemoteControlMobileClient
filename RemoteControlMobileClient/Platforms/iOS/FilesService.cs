namespace RemoteControlMobileClient.BusinessLogic.Services.Partial
{
    internal partial class FilesService
    {
        public partial Task<bool> SaveFileAsync(byte[] fileBytes, string fileName)
        {
            return Task.FromResult(false);
        }

		public partial Task<bool> SaveFileAsync(string fileName, Stream stream, IProgress<double> progress = null, CancellationToken token = default)
        {
            return Task.FromResult(false);
        }

		public partial string CreateUniqueDownloadPath(string fileName)
        {
            return string.Empty;
        }

	}
}
