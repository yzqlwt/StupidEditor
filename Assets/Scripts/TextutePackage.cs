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

    public class TextutePackage : MonoBehaviour
    {
        // Start is called before the first frame update
        private List<ResourceInfo> TotalResInfo;
        void Start()
        {
            TypeEventSystem.Register<ExportCommand>((tex)=> {
                if(tex.command == SequenceCommand.TexturePackage)
                {
                    TotalResInfo = tex.TotalResInfo.Where((info) => {
                        return info.Tag == ResourceTag.TexturePackage;
                    }).ToList();
                    if (!isCanFit())
                    {
                        TypeEventSystem.Send(new ExportCommandDone()
                        {
                            Ret = false,
                            Reason = "图片总面积超过了2048*2048，建议把背景图标记为不合图。\n下个版本会支持拆分为多个plist,感谢支持！"
                        });
                    }
                    else
                    {
                        var tobePackedPath = DirTools.GetTobePackedTexuresPath("default");
                        TotalResInfo.ForEach((info) =>
                        {
                            File.Copy(info.FileFullName, tobePackedPath+"/"+info.FileName);
                        });
                        TypeEventSystem.Send(new ExportCommandDone() {
                            Ret = true,
                            Reason = "合图完成"
                        });
                    }
                }

            });
        }

        bool isCanFit()
        {
            var totalSize = TotalResInfo.Select((info) =>
            {
                return info.Width * info.Height;
            }).Sum();
            return totalSize < 2048 * 2048;
        }
        void TexturePackageProcess(string resDir, string outputDir, string name)
        {
            var command = Application.streamingAssetsPath + "/TexturePackor/TexturePacker.exe";
            var argu = string.Format(@"{0} --sheet {1}/{2}.png --data {1}/{2}.plist --allow-free-size --no-trim --max-size 2048 --format cocos2d", resDir, outputDir, name);
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
    }
}
