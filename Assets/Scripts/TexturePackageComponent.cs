using System.Diagnostics;
using System.IO;

namespace StupidEditor
{
    using System.Collections;
    using System.Collections.Generic;
    using QFramework;
    using UnityEngine;
    using System.Linq;
    using SimplePopup;
    using QFramework.Example;
    using System.Threading;

    public class TexturePackageDone : ExportCommandDone
    {
        public List<string> PlistsName;
        public List<string> Files;
    }
    public class TexturePackageComponent : MonoBehaviour
    {
        // Start is called before the first frame update
        private List<ResourceInfo> TotalResInfo;
        private string PlistName = "__frame_1";
        void Start()
        {
        }

        bool isCanFit()
        {
            var totalSize = TotalResInfo.Select((info) =>
            {
                return info.Width * info.Height;
            }).Sum();
            return totalSize < 2048 * 2048;
        }
        void TexturePackageCommand(string resDir, string outputDir, string plistName)
        {
            var command = Application.streamingAssetsPath + "/TexturePackor/TexturePacker.exe";
            var argu = string.Format(@"{0} --sheet {1}/{2}.png --data {1}/{2}.plist --allow-free-size --no-trim --max-size 2048 --format cocos2d", resDir, outputDir, plistName);
            using (Process process = new Process())
            {
                process.StartInfo.FileName = command;

                process.StartInfo.Arguments = argu;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();

                StreamReader reader = process.StandardOutput;
                string output = reader.ReadToEnd();
                process.WaitForExit();
                reader.Close();
                Debug.Log(output);
            }
        }

        public TexturePackageDone TexturePackage(List<ResourceInfo> totalInfo)
        {
            DirTools.DeleteFilesAndFolders(DirTools.GetTobePackedTexuresPath());
            var textureInfo = TotalResInfo = totalInfo.Where((info) =>
            {
                return info.Tag == ResourceTag.TexturePackage ||
                       (info.Tag == ResourceTag.CocosStudio && info.Extension == ".png");
            }).ToList();
            if (!isCanFit())
            {
                return new TexturePackageDone()
                {
                    Ret = false,
                    Reason = "图片总面积超过了2048*2048，建议把背景图标记为不合图。\n下个版本会支持拆分为多个plist,感谢支持！"
                };
            }
            else if (textureInfo.Count == 0)
            {
                return new TexturePackageDone()
                {
                    Ret = true,
                    Reason = "合图完成",
                    PlistsName = new List<string>(),
                    Files = new List<string>(),
                };
            }
            else
            {
                var tobePackedPath = DirTools.GetTobePackedTexuresPath();
                textureInfo.ForEach((info) =>
                {
                    var filePath = tobePackedPath + "/" + info.MD5 + info.Extension;
                    File.Copy(info.FileFullName, filePath, true);
                });
                TexturePackageCommand(tobePackedPath, tobePackedPath, PlistName);
                return new TexturePackageDone()
                {
                    Ret = true,
                    Reason = "合图完成",
                    PlistsName = new List<string>()
                    {
                        PlistName
                    },
                    Files = new List<string>()
                    {
                        tobePackedPath + "/" + PlistName + ".png",
                        tobePackedPath + "/" + PlistName + ".plist",
                    }
                };
            }
        }
    }
}
