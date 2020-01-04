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
                        //SimplePopupManager.Instance.CreatePopup("图片总面积超过了2048*2048，建议把背景图标记为不合图。\n下个版本会支持拆分为多个plist,感谢支持！");
                        //SimplePopupManager.Instance.AddButton("朕知道,退下吧", delegate { Debug.Log("clicked on yes"); });
                        //SimplePopupManager.Instance.ShowPopup();
                        TypeEventSystem.Send(new ExportCommandDone()
                        {
                            Ret = false,
                            Reason = "图片总面积超过了2048*2048，建议把背景图标记为不合图。\n下个版本会支持拆分为多个plist,感谢支持！"
                        });
                    }
                    else
                    {
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
        void TexturePackageProcess()
        {

        }
    }
}
