using MetroTrilithon.Serialization;
using System;
using System.IO;

namespace WinCap.Models.Settings
{
    public static class Providers
    {
        public static string LocalFilePath { get; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "thirdplay", "WinCap", "Settings.xaml");

        public static ISerializationProvider Local { get; } = new FileSettingsProvider(LocalFilePath);
    }
}
