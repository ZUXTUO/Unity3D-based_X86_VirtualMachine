using System.Configuration;

namespace x86CS.Configuration
{
    public static class SystemConfig
    {
        public static Machine machine;
    }

    public enum DriveType
    {
        None,
        HardDisk,
        CDROM
    }
}
