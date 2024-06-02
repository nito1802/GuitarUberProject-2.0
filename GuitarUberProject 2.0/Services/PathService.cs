using GitarUberProject;
using System.IO;

namespace GuitarUberProject_2._0.Services
{
    public static class PathService
    {
        public static string GetBasePathToRecords()
        {
            string recordPath = Path.Combine(App.FolderSettingsPath, "lastRecorded");
            if (!Directory.Exists(recordPath)) Directory.CreateDirectory(recordPath);
            return recordPath;
        }
    }
}