namespace RemoteControlMobileClient.BusinessLogic.Services.Partial
{
    internal partial class FilesService
    {
        public partial Task<bool> SaveFileAsync(byte[] fileBytes, string fileName);
    }
}
