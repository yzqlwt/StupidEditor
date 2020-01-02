namespace StupidEditor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using QFramework;
    using System.IO;
    using System.Text;
    using System.Linq;

    public class FileDragIn
    {
        public string FileFullName;
    }
    public class FileInfo
    {
        public string FileName;
        public string FileFullName;
        public string Extension;
        public string MD5;
        public DateTime Time;
    }

    public class FileReceiver : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            DirTools.ClearTempPath();
            TypeEventSystem.Register<FileDragIn>((fileDragIn)=> {
                var fileFullName = fileDragIn.FileFullName;
                System.IO.FileInfo file = new System.IO.FileInfo(fileFullName);
                var tempPath = DirTools.GetTempPath();
                File.Copy(file.FullName, tempPath + "/" + file.Name, true);
                var md5Code = GetMD5HashFromFile(fileFullName);
                TypeEventSystem.Send(new FileInfo()
                {
                    FileName = file.Name,
                    FileFullName = tempPath + "/" + file.Name,
                    Extension = file.Extension,
                    Time = DateTime.Now,
                    MD5 = md5Code
                });

            });

            var files = Directory.GetFiles(@"C:\Users\yzqlwt\Pictures\互动2-1_slices", "*").Where(s => s.EndsWith(".jpg") || s.EndsWith(".png"));
            var i = 0;
            foreach (var file in files)
            {
                if (i > 10)
                {
                    break;
                }
                TypeEventSystem.Send(new FileDragIn()
                {
                    FileFullName = file
                });
                if (file.IndexOf("2") > 0)
                {
                    i++;
                }

            }
        }

        /// <summary>
        /// Gets the MD5 hashCode from file.
        /// </summary>
        /// <returns>The MD5 hashCode from file.</returns>
        /// <param name="fileFullName">File full name.</param>
        public  string GetMD5HashFromFile(string fileFullName)
        {
            try
            {
                FileStream file = new FileStream(fileFullName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
    }
}

