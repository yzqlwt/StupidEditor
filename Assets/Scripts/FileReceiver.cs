using B83.Win32;
using SimplePopup;

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
        public string Tag;
        public POINT Point = new POINT(0,0);
    }

    public enum DragDropType
    {
        Add,
        Replace,
    }
    public class FileInfo
    {
        public string FileName;
        public string FileFullName;
        public string Extension;
        public string MD5;
        public DateTime Time;
        public string Tag;
        public DragDropType DropType;
    }

    public class FileReceiver : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            DirTools.ClearTempPath();
            TypeEventSystem.Register<FileDragIn>((fileDragIn)=> {
                var path = fileDragIn.Path;
                System.IO.FileInfo file = new System.IO.FileInfo(path);
                var tempPath = DirTools.GetTempPath();
                File.Copy(file.FullName, tempPath + "/" + file.Name, true);
                var md5Code = GetMD5HashFromFile(path);
                if (md5Code == null)
                {
                    SimplePopupManager.Instance.CreatePopup(string.Format("{0}可能被占用",fileDragIn.Path));
                    SimplePopupManager.Instance.AddButton("朕知道了", delegate {  });
                    SimplePopupManager.Instance.ShowPopup();
                    return;
                }
                TypeEventSystem.Send(new FileInfo()
                {
                    FileName = file.Name,
                    FileFullName = tempPath + "/" + file.Name,
                    Extension = file.Extension,
                    Time = DateTime.Now,
                    MD5 = md5Code,
                    Tag = fileDragIn.Tag,
                    DropType = fileDragIn.Point.x < 680 ? DragDropType.Add : DragDropType.Replace,
                });
            });
            if(Application.platform == RuntimePlatform.WindowsEditor )
            {
                TypeEventSystem.Send(new FileDragIn()
                {
                    Path = @"C:\Users\yzqlw\Desktop\ShadowsocksR-win-4.9.0\ShadowsocksR-dotnet4.0.exe",
                    Tag  = ResourceTag.TexturePackage,
                    Point = new POINT(400, 300)
                });
            }else if(Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
            {
                TypeEventSystem.Send(new FileDragIn()
                {
                    Path = @"/Users/yzqlwt/Desktop/image"
                });
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
                return null;
            }
        }
    }
}

