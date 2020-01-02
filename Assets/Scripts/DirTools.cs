using System.IO;

namespace StupidEditor 
{ 
    public class DirTools 
    {
        /// <summary>
        /// Gets the desktop path.
        /// </summary>
        /// <returns>The desktop path.</returns>
        public static string GetDesktopPath()
        { 
            return System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
        }

        /// <summary>
        /// Gets the base path.
        /// </summary>
        /// <returns>The base path.</returns>
        public static string GetBasePath()
        {
            var desktopPath = GetDesktopPath();
            var path = desktopPath + "/StupidEditorData";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Gets the temp path.
        /// </summary>
        /// <returns>The temp path.</returns>
        public static string GetTempPath()
        {
            var basePath = GetBasePath();
            var path = basePath + "/Temp";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Clears the temp path.
        /// </summary>
        public static void ClearTempPath()
        {
            var tempPath = GetTempPath();
            Directory.Delete(tempPath, true);
            Directory.CreateDirectory(tempPath);
        }
    }
}