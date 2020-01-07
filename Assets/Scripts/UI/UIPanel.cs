//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.IO;

namespace QFramework.Example
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;
    using QFramework;
    using StupidEditor;
    using System.Threading.Tasks;
    using System.Threading;
    using SimplePopup;
    public class ExportCommandDone
    {
        public bool Ret;
        public string Reason;
    }

    public class UIPanelData : QFramework.UIPanelData
    {
    }

    public partial class UIPanel : QFramework.UIPanel
    {

        public GameObject ResourceItem;
        public Transform ScrollViewContent;
        public Transform Inspector;
        protected override void ProcessMsg(int eventId, QFramework.QMsg msg)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnInit(QFramework.IUIData uiData)
        {
            mData = uiData as UIPanelData ?? new UIPanelData();
            // please add init code here
            TypeEventRegister();
        }

        protected override void OnOpen(QFramework.IUIData uiData)
        {
        }

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
        }
        private void TypeEventRegister()
        {
            TypeEventSystem.Register<FileInfo>((fileInfo) =>
            {
                
                List<Transform> children = new List<Transform>();
                foreach (Transform child in ScrollViewContent)
                {
                    // var info = child.GetComponent<ResourceItem>().ResInfo;
                    children.Add(child);
                }

                var target = children.Find((child) =>
                {
                    var info = child.GetComponent<ResourceItem>().ResInfo;
                    return info.FileName == fileInfo.FileName;
                });
                if (target != null)
                {
                    SimplePopupManager.Instance.CreatePopup(string.Format("文件名相同，直接替换原有资源{0}", fileInfo.FileName));
                    SimplePopupManager.Instance.AddButton("知道了", delegate
                    {
                        target.GetComponent<ResourceItem>().SetItemInfo(fileInfo);
                    });
                    SimplePopupManager.Instance.ShowPopup();
                    return;
                }
                Debug.Log(fileInfo.DropType);
                if (fileInfo.DropType == DragDropType.Add)
                {
                    var Item = Instantiate(ResourceItem, ScrollViewContent);
                    Item.GetComponent<ResourceItem>().SetItemInfo(fileInfo);
                    Item.GetComponent<Toggle>().group = ScrollViewContent.GetComponent<ToggleGroup>(); 
                }
                else
                {
                    var inspector = Inspector.GetComponent<Inspector>();
                    if (!inspector.Mask.activeSelf)
                    {
                        ///替换item
                        var item = inspector.SelectItem;
                        if (item)
                        {
                            var resItem = item.GetComponent<ResourceItem>();
                            if (resItem.ResInfo.Extension != fileInfo.Extension)
                            {
                                SimplePopupManager.Instance.CreatePopup(string.Format("不可以把 {0} 替换为 {1}", resItem.ResInfo.Extension, fileInfo.Extension));
                                SimplePopupManager.Instance.AddButton("Soga", delegate { });
                                SimplePopupManager.Instance.ShowPopup();
                                File.Delete(fileInfo.FileFullName);
                                return;
                            }
                            SimplePopupManager.Instance.CreatePopup(string.Format("确定替换 {0}{1}", inspector.InputName.text, inspector.ExtensionText.text));
                            SimplePopupManager.Instance.AddButton("嗯嗯", delegate {
                                if (File.Exists(resItem.ResInfo.FileFullName))
                                {
                                    var originName = resItem.ResInfo.FileName;
                                    File.Delete(resItem.ResInfo.FileFullName);
                                    resItem.SetItemInfo(fileInfo);
                                    resItem.SetFileName(originName);
                                }
                                else
                                {
                                    Debug.LogError("替换文件，被替换的文件不存在");
                                }
                                Debug.Log(resItem.ResInfo);
                            });
                            SimplePopupManager.Instance.AddButton("算了算了", delegate
                            {
                                File.Delete(fileInfo.FileFullName);
                            });
                            SimplePopupManager.Instance.ShowPopup();
                        }

                    }
                    else
                    {
                        SimplePopupManager.Instance.CreatePopup("没有选中的资源，无法进行替换");
                        SimplePopupManager.Instance.AddButton("朕晓得了", delegate {  });
                        SimplePopupManager.Instance.ShowPopup();
                    }
                }


            });
        }

        public async void Export()
        {
            Debug.Log("导出");
            DirTools.ClearOutputPath();
            List<ResourceInfo> TotalInfo = new List<ResourceInfo>();
            foreach (Transform child in ScrollViewContent)
            {
                var info = child.GetComponent<ResourceItem>().ResInfo;
                TotalInfo.Add(info);
            }

            var texturePackageComponent = transform.GetComponent<TexturePackageComponent>();
            var result = texturePackageComponent.TexturePackage(TotalInfo);
            if (result.Ret == false)
            {
                ShowErrorTips(result.Reason);
                return;
            }
            else
            {
                var resTexturePackage = (TexturePackageDone) result;
                var generateJsonComponent = transform.GetComponent<GenerateJsonComponent>();
                var result1 = generateJsonComponent.SerializeToJson(TotalInfo, resTexturePackage.PlistsName);
                if (result1.Ret == false)
                {
                    ShowErrorTips(result1.Reason);
                    return;
                }
                var resJson = (GenerateJsonDone) result1;
                var NonePath = DirTools.GetOutputNonePath();
                var PlistPath = DirTools.GetOutputPlistPath();
                var CsbPath = DirTools.GetOutputCsbPath();
                var OutPutPath = DirTools.GetOutputPath();
                var filesNone = TotalInfo.Where((info) => { return info.Tag == ResourceTag.None; }).ForEach((info) =>
                {
                    File.Copy(info.FileFullName, NonePath+"/"+info.FileName);
                });
                var filesCsb = TotalInfo.Where((info) => { return info.Tag == ResourceTag.CocosStudio; }).ForEach((info) =>
                {
                    File.Copy(info.FileFullName, CsbPath+"/"+info.FileName);
                });
                resTexturePackage.Files.ForEach((path) =>
                {
                    var fileName = System.IO.Path.GetFileName(path);
                    File.Copy(path, PlistPath+"/"+fileName);
                });
                resJson.Files.ForEach((path) =>
                {
                    var fileName = System.IO.Path.GetFileName(path);
                    File.Copy(path, OutPutPath+"/"+fileName);
                });
                ZipUtil.ZipDirectory(DirTools.GetOutputPath(),DirTools.GetBasePath()+"/res.zip");
                System.Diagnostics.Process.Start(DirTools.GetBasePath());
            }
   

        }
        void ShowErrorTips(string reason)
        {
            SimplePopupManager.Instance.CreatePopup(reason);
            SimplePopupManager.Instance.AddButton("朕知道,退下吧", delegate { Debug.Log("clicked on yes"); });
            SimplePopupManager.Instance.ShowPopup();
        }

        public void Thank()
        {
            SimplePopupManager.Instance.CreatePopup("还没想好往上放什么");
            SimplePopupManager.Instance.AddButton("朕知道,退下吧", delegate { Debug.Log("clicked on yes"); });
            SimplePopupManager.Instance.ShowPopup();
        }
        
    }
}