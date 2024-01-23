namespace RemoteControlMobileClient.BusinessLogic.Models
{
    internal class Device
    {
        public int Id { get; set; }

        public string DeviceName { get; set; }

        public string DeviceType { get; set; }

        public string DevicePlatform { get; set; }

        public string DevicePlatformVersion { get; set; }

        public string DeviceManufacturer { get; set; }

        public bool IsConnected { get; set; }
    }
}
