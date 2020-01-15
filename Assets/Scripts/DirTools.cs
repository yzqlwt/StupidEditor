using System.IO;

namespace StupidEditor 
{ 
    public class DirTools 
    {
        
        
        /// <summary>
        /// 递归删除文件夹内所有的文件以及文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteFilesAndFolders(string path)
        {
            // Delete files.
            string[] files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                File.Delete(file);
            }

            // Delete folders.
            string[] folders = Directory.GetDirectories(path);
            foreach (var folder in folders)
            {
                DeleteFilesAndFolders(folder);
                Directory.Delete(folder);
            }
        }
        
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
            DeleteFilesAndFolders(tempPath);
            Directory.CreateDirectory(tempPath);
        }

        public static string GetTobePackedTexuresPath()
        {
            var tempPath = GetTempPath();
            var path = tempPath + "/ToBePacked";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
        public static string GetTempConfigPath()
        {
            var tempPath = GetTempPath();
            var path = tempPath + "/TempConfig";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public static void ClearOutputPath()
        {
            var outputPath = GetOutputPath();
            DeleteFilesAndFolders(outputPath);
            Directory.CreateDirectory(outputPath);
        }
        public static string GetOutputPath()
        {
            var basePath = GetBasePath();
            var path = basePath + "/Output";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
        public static string GetOutputPlistPath()
        {
            var outputPath = GetOutputPath();
            var path = outputPath + "/plist";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
        public static string GetOutputNonePath()
        {
            var outputPath = GetOutputPath();
            var path = outputPath + "/none";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
        public static string GetOutputCsbPath()
        {
            return GetOutputPath();
        }

        public static string GetSplitedPNGDir()
        {
            var tempPath = GetTempPath();
            var path = tempPath + "/SplitedPNGDir";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public static string GetRestoredPNGDir()
        {
            var tempPath = GetTempPath();
            var path = tempPath + "/RestoredPNGDir";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
        public static string GetDownloadDir()
        {
            var tempPath = GetTempPath();
            var path = tempPath + "/download";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}