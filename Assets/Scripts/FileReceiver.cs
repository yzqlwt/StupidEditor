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
        public string Path;
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
                var path = fileDragIn.Path;
                if (File.Exists(path))
                {
                    System.IO.FileInfo file = new System.IO.FileInfo(path);
                    var tempPath = DirTools.GetTempPath();
                    File.Copy(file.FullName, tempPath + "/" + file.Name, true);
                    var md5Code = GetMD5HashFromFile(path);
                    TypeEventSystem.Send(new FileInfo()
                    {
                        FileName = file.Name,
                        FileFullName = tempPath + "/" + file.Name,
                        Extension = file.Extension,
                        Time = DateTime.Now,
                        MD5 = md5Code
                    });
                }
                else if (Directory.Exists(path))
                {
                    Directory.GetFiles(path, "*").ForEach((file) => {
                        TypeEventSystem.Send(new FileDragIn()
                        {
                            Path = file
                        });
                    });

                }
                else
                {
                    Debug.LogError("ERROR FileDragIn"+ path);
                }


            });
            TypeEventSystem.Send(new FileDragIn()
            {
                Path = @"C:\Users\yzqlwt\Pictures\互动2-1_slices"
            });

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

