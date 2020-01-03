namespace StupidEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using QFramework;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class ResourceTag
    {
        public static string CocosStudio = "CocosStudio(不合图不做处理)";
        public static string TexturePackage = "进行合图(适用于大部分资源)";
        public static string UnTexturePackage = "不进行合图(适用于较大的背景图)";
    }

    public class ResourceInfo
    {
        public string FileName;
        public string FileFullName;
        public string Extension;
        public string MD5;
        public DateTime Time;
        public ResourceInfo(FileInfo info)
        {
            FileName = info.FileName;
            FileFullName = info.FileFullName;
            Extension = info.Extension;
            MD5 = info.MD5;
            Time = info.Time;
        }

        //
        public string Tag = "";
    }

    public class ResourceItem : MonoBehaviour, IPointerClickHandler
    {
        private ResLoader mResLoader;
        public Text FileName;
        public Image FileImage;
        public Image TagImage;
        public ResourceInfo ResInfo { private set; get;}
        // Start is called before the first frame update
        void Start()
        {
            mResLoader = ResLoader.Allocate();
        }

        public void SetItemInfo(FileInfo info)
        {
            ResInfo = new ResourceInfo(info);
            SetUI();
        }
        public void SetUI()
        {
            FileName.text = ResInfo.FileName;

            StartCoroutine(GetImage(ResInfo));
        }

        IEnumerator GetImage(ResourceInfo info)
        {
            var size = FileImage.GetComponent<RectTransform>().sizeDelta;
            var whRatio = size.x / size.y;
            var path = "";
            if (info.Extension == ".png" || info.Extension == ".jpg")
            {
                path = @"file://" + info.FileFullName;
            }
            else if (info.Extension == ".swf")
            {
                path = @"file://" + Application.streamingAssetsPath + "/texture/swf.png";
            }
            else
            {
                path = @"file://" + Application.streamingAssetsPath + "/texture/unknown.png";
            }
#pragma warning disable CS0618 // 类型或成员已过时
            var www = new WWW(path);
#pragma warning restore CS0618 // 类型或成员已过时
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                Texture2D tex = www.texture;
                if(tex.width/tex.height > whRatio)
                {
                    var y = size.x / tex.width * tex.height;
                    FileImage.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, y);
                }
                else
                {
                    var x = size.y / tex.height * tex.width;
                    FileImage.GetComponent<RectTransform>().sizeDelta = new Vector2(x, size.y);
                }
                Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
                FileImage.sprite = temp;
            }
            StartCoroutine(AddTag(info));

        }
        IEnumerator AddTag(ResourceInfo info)
        {
            string path = "";
            if(info.Tag == ResourceTag.TexturePackage)
            {
                path = @"file://" + Application.streamingAssetsPath + "/texture/subscript/green.png";
                TagImage.gameObject.SetActive(false);
            }
            else if(info.Tag == ResourceTag.CocosStudio)
            {
                path = @"file://" + Application.streamingAssetsPath + "/texture/subscript/red.png";
                TagImage.gameObject.SetActive(true);
            }
            else if (info.Tag == ResourceTag.UnTexturePackage)
            {
                path = @"file://" + Application.streamingAssetsPath + "/texture/subscript/yellow.png";
                TagImage.gameObject.SetActive(true);
            }
#pragma warning disable CS0618 // 类型或成员已过时
            var www = new WWW(path);
#pragma warning restore CS0618 // 类型或成员已过时
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                Texture2D tex = www.texture;
                Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
                TagImage.sprite = temp;
            }
            else
            {
                UnityEngine.Debug.Log(www.error);
            }
        }

        void Destroy()
        {
            // 释放所有本脚本加载过的资源
            // 释放只是释放资源的引用
            // 当资源的引用数量为 0 时，会进行真正的资源卸载操作
            mResLoader.Recycle2Cache();
            mResLoader = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount == 2)
            {
                if(Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = "open";
                    psi.Arguments = ResInfo.FileFullName;
                    psi.UseShellExecute = false;
                    psi.RedirectStandardOutput = true;
                    Process p = Process.Start(psi);
                    string strOutput = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();
                    UnityEngine.Debug.Log(strOutput);
                }
                else if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    process.StartInfo.FileName = ResInfo.FileFullName;
                    process.StartInfo.Arguments = "rundll32.exe C://WINDOWS//system32//shimgvw.dll";
                    process.StartInfo.UseShellExecute = true;
                    process.Start();
                }



            }
            if(eventData.button == PointerEventData.InputButton.Left)
            {

                StartCoroutine(ClickResourceItem());
            }
            
        }
        IEnumerator ClickResourceItem()
        {
            yield return new WaitForFixedUpdate();
            TypeEventSystem.Send(new ResourceItemClick());
        }

        public void SetFileName(string name)
        {
            ResInfo.FileName = name;
            FileName.text = name;
            System.IO.FileInfo file = new System.IO.FileInfo(ResInfo.FileFullName);
            file.MoveTo(file.DirectoryName+"/"+name);
            ResInfo.FileFullName = file.DirectoryName + "/" + name;
        }
        public void SetTag(string ret)
        {
            ResInfo.Tag = ret;
            StartCoroutine(AddTag(ResInfo));
        }
        public void DeleteItem()
        {
            if (File.Exists(ResInfo.FileFullName))
            {
                File.Delete(ResInfo.FileFullName);
            }
            Destroy(gameObject);
        }
    }
}
