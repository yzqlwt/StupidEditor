using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using B83.Win32;
using DebounceThrottle;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NRatel.TextureUnpacker;
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
    using System.Threading.Tasks;

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
                if (file.Extension == ".zip")
                {
                    ImportZip(path);
                    return;
                }
                else if (file.Extension == ".csb")
                {
                    // var csbInspector = transform.GetComponent<CsbInspector>();
                    // var totalInfo = transform.GetComponent<QFramework.Example.UIPanel>().GetTotalInfo();
                    // totalInfo = totalInfo.Where((info) => { return info.Extension == ".csb"; }).ToList();
                    // var paths = totalInfo.Select((info) => { return info.FileFullName; }).ToList();
                    // paths.Add(path);
                    // var totalPaths = new List<string>();
                    // paths.ForEach((name) =>
                    // {
                    //     totalPaths.AddRange(csbInspector.GetCsbImagePath(name));
                    // });
                    // totalPaths = totalPaths.Select((t) => { return Path.GetFileName(t); }).ToList();
                    // totalPaths = totalPaths.Distinct().ToList();
                    // transform.GetComponent<AppData>().CsbImagePaths = totalPaths;
                }
                else if(file.Extension == ".png")
                {
                    // var ret = isCocosStudioImage(path);
                    // if (ret)
                    // {
                    //     fileDragIn.Tag = ResourceTag.CocosStudio;
                    // }
                }
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
                    DropType = fileDragIn.Point.x < 860 ? DragDropType.Add : DragDropType.Replace,
                });
            });
            if(Application.platform == RuntimePlatform.WindowsEditor )
            {
                var path = @"C:\Users\yzqlw\Desktop\png";
                Directory.GetFiles(path, "*").ForEach((file) =>
                {
                    TypeEventSystem.Send(new FileDragIn()
                    {
                        Path = file,
                        Tag = ResourceTag.Default
                    });
                });
                // TypeEventSystem.Send(new FileDragIn()
                // {
                //     Path = path,
                //     Tag = ResourceTag.Default
                // });
                Invoke("test", 3.0f);
            }else if(Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
            {
                var path = @"/Users/yzqlwt/Desktop/image";
                Directory.GetFiles(path, "*").ForEach((file) => {
                    TypeEventSystem.Send(new FileDragIn()
                    {
                        Path = file,
                        Tag = ResourceTag.Default,
                        Point = new POINT(900,100)
                    });
                });
            }
        }

        bool isCocosStudioImage(string path)
        {
            var filename = Path.GetFileName(path);
            var paths = transform.GetComponent<AppData>().CsbImagePaths;
            return paths.Contains(filename);
        }

        void test()
        {
            // var path = @"C:\Users\yzqlw\Desktop\G105Layer.csb";
            // var path2 = @"C:\Users\yzqlwt\Dropbox\unity\csb2csd-master\G105Layer.csb";
            // TypeEventSystem.Send(new FileDragIn()
            // {
            //     Path = path,
            //     Tag = ResourceTag.Default,
            //     Point = new POINT(200,100)
            // });
            // TypeEventSystem.Send(new FileDragIn()
            // {
            //     Path = @"C:\Users\yzqlw\Desktop\base_side.png",
            //     Tag = ResourceTag.Default,
            //     Point = new POINT(200,100)
            // });
        }

        void ImportZip(string path)
        {
            var tempPath = DirTools.GetTempPath();
            var unzipPath = tempPath + "/unzip";
            if(Directory.Exists(unzipPath))
                DirTools.DeleteFilesAndFolders(unzipPath);
            ZipUtil.UnZipFile(path, unzipPath);
            var isExist = File.Exists(unzipPath + "/ResConfig.json");
            var ret = false;
            if (isExist)
            {
                StreamReader sr = new StreamReader(unzipPath + "/ResConfig.json");
                if (sr == null)
                {
                    return;
                }
                string json = sr.ReadToEnd();
                sr.Close();
                var configTemplate = JsonConvert.DeserializeObject<ConfigTemplate>(json);
                if (configTemplate.resource!=null)
                {
                    ret = true;
                }
            }
            if (ret)
            {
                StreamReader sr = new StreamReader(unzipPath + "/ResConfig.json");
                if (sr == null)
                {
                    return;
                }
                string json = sr.ReadToEnd();
                sr.Close();
                var configTemplate = JsonConvert.DeserializeObject<ConfigTemplate>(json);
                configTemplate.resource.Where((item) =>
                {
                    return item.Value.Tag != ResourceTag.TagsMap[ResourceTag.TexturePackage];
                }).ForEach((item) =>
                {
                    var tag = ResourceTag.TagsMap.Where((tagItem) => { return tagItem.Value == item.Value.Tag; })
                        .First().Key;
                    string filepath = "";
                    if (tag == ResourceTag.None)
                    {
                        filepath = unzipPath + "/none/" + item.Value.Name;
                    }else if (tag == ResourceTag.CocosStudio)
                    {
                        filepath = unzipPath + "/" + item.Value.Name;
                    }
                    Debug.Log(string.Format("从zip导入文件路径{0} 文件Tag{1}", filepath, tag));
                    TypeEventSystem.Send(new FileDragIn()
                    {
                        Path = filepath,
                        Tag = tag
                    });
                });
                if (configTemplate.plist != null)
                {
                    configTemplate.plist.ForEach((name) =>
                    {
                        var plistPath = unzipPath + "/plist/" + name + ".plist";
                        var pngPath = unzipPath + "/plist/" + name + ".png";
                        Unpacker(plistPath, pngPath);
                    });
                    configTemplate.resource.Where((item) =>
                    {
                        return item.Value.Tag == ResourceTag.TagsMap[ResourceTag.TexturePackage];
                    }).ForEach((item) =>
                    {
                        var md5 = item.Value.Md5;
                        string imagePath = DirTools.GetRestoredPNGDir() + "/" + md5 + item.Value.Extension;
                        Debug.Log(imagePath);
                        if (File.Exists(imagePath))
                        {
                            File.Move(imagePath, DirTools.GetRestoredPNGDir() + "/" + item.Value.Name);
                            TypeEventSystem.Send(new FileDragIn()
                            {
                                Path = DirTools.GetRestoredPNGDir() + "/" + item.Value.Name,
                                Tag = ResourceTag.TexturePackage
                            });
                        }
                        else
                        {
                            Debug.Log("bububu");
                        }
                    });
                }
            }
            else
            {
                Directory.GetFiles(unzipPath, "*").ForEach((file) =>
                {
                    var extension = System.IO.Path.GetExtension(file);
                    if (extension != ".plist")
                    {
                        TypeEventSystem.Send(new FileDragIn()
                        {
                            Path = file,
                            Tag = ResourceTag.Default
                        });
                    }
                    else
                    {
                        var pathName = System.IO.Path.GetDirectoryName(file);
                        var fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                        var plistPath = file;
                        var pngPath = pathName + '/'+fileName + ".png"; 
                        Unpacker(plistPath, pngPath);
                        var RestoredPath = DirTools.GetRestoredPNGDir();
                        Directory.GetFiles(RestoredPath, "*").ForEach((filePath) =>
                        {
                            TypeEventSystem.Send(new FileDragIn()
                            {
                                Path = filePath,
                                Tag = ResourceTag.Default
                            });
                        });
                    }

                });
            }

        }

        void Unpacker(string plistFilePath, string pngFilePath)
        {
            DirTools.DeleteFilesAndFolders(DirTools.GetRestoredPNGDir());
            var loader = NRatel.TextureUnpacker.Loader.LookingForLoader(plistFilePath);
            if (loader != null)
            {
                var plist = loader.LoadPlist(plistFilePath);
                var bigTexture = loader.LoadTexture(pngFilePath, plist.metadata);

                int total = plist.frames.Count;
                int count = 0;
                foreach (var frame in plist.frames)
                {
                    try
                    {
                        Core.Restore(bigTexture, frame);
                        count += 1;
                    }
                    catch
                    {
                    }
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
                return null;
            }
        }
    }
}

