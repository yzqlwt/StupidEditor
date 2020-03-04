using System;
using System.IO;
using System.Linq;
using B83.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using QFramework;
using SimplePopup;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace StupidEditor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    class Attachments{
        public string uri;
        public string name;
        public string ext_name;
    }
    class Attachment
    {
        public int id;
        public string name;
        public Attachments attachments;
    }

    public class Backends : MonoBehaviour
    {
        // Start is called before the first frame update
        private Template _template;
        public Transform ScrollViewContent;
        public Transform SkinsPanel;
        public Transform SkinItem;
        public Transform HttpProgress;
        private string ResName = "ResConfig";
        private string _attachmentId = null;
        private string _operation = "";
        private string _zipname = "";
        private void Start()
        {
            SkinsPanel.gameObject.SetActive(false);
            // var httpProgress = Instantiate(HttpProgress, transform);
            // httpProgress.GetComponent<WebRequest>().StartRequest(url, zipPath);
        }

        public void BtnClose()
        {
            SkinsPanel.gameObject.SetActive(false);
        }
        public string GetAuthCode()
        {
            var token = JsonConvert.DeserializeObject<AccessTokenStruct>(PlayerPrefs.GetString("token"));
            return token.access_token;
        }

        public void Upload(Template template)
        {
            _template = template;
            SkinsPanel.gameObject.SetActive(true);
            _operation = "upload";
            StartCoroutine(GetSkins(template.id));
        }
        public void Download(Template template)
        {
            _template = template;
            _operation = "download";
            SkinsPanel.gameObject.SetActive(true);
            StartCoroutine(GetSkins(template.id));
        }
        public IEnumerator GetSkins(int template_id)
        {
            var url = string.Format(AppConfig.JiuquUrl+"/admin-course/skin?templateId={0}", template_id);
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            webRequest.SetRequestHeader("Authorization", GetAuthCode());
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                var skinsConfig = JsonConvert.DeserializeObject<List<SkinConfig>>(webRequest.downloadHandler.text);
                List<Transform> children = new List<Transform>();
                foreach (Transform child in ScrollViewContent)
                {
                    children.Add(child);
                }
                
                children.ForEach((child) =>
                {
                    Destroy(child.gameObject);
                });
                skinsConfig.ForEach((skin) =>
                {
                    var item = Instantiate(SkinItem, ScrollViewContent);
                    item.GetComponent<SkinItemScript>().SetSkin(skin);
                    item.GetComponent<Toggle>().group = ScrollViewContent.GetComponent<ToggleGroup>();
                });
            }
        }
        
        IEnumerator GetAttachments(string itemType, int itemId)
        {
            var url = string.Format(AppConfig.JiuquUrl+"/admin-course/item/attachment?itemType={0}&itemId={1}", itemType, itemId);
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            webRequest.SetRequestHeader("Authorization", GetAuthCode());
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                var list = JsonConvert.DeserializeObject<List<Attachment>>(webRequest.downloadHandler.text);
                var attachment = list.Find((item) => item.name == ResName);
                if (_operation == "upload")
                {
                    yield return StartCoroutine(DownloadZip(attachment));
                    yield return StartCoroutine(UploadZip(itemId));
                    var attachmentId = _attachmentId;
                    if (attachment != null)
                    {
                    
                        yield return StartCoroutine(UpdateArgu(attachment.id.ToString(), itemType, itemId.ToString(),
                            attachment.name, attachmentId));
                    }
                    else
                    {
                        yield return StartCoroutine(CreateResArgu(itemType, itemId, attachmentId));
                    }

                    SimplePopupManager.Instance.CreatePopup("上传成功~~~");
                    SimplePopupManager.Instance.AddButton("朕知道,退下吧", delegate { Debug.Log("clicked on yes"); });
                    SimplePopupManager.Instance.ShowPopup();
                }
                else if (_operation == "download")
                {
                    if (attachment != null)
                    {

                        yield return StartCoroutine(DownloadZip(attachment));
                        TypeEventSystem.Send(new FileDragIn()
                        {
                            Path = _zipname,
                            Tag = ResourceTag.Default,
                            Point = new POINT(0,0)
                        });
                    }
                    else
                    {
                        SimplePopupManager.Instance.CreatePopup("后台没有"+ResName);
                        SimplePopupManager.Instance.AddButton("朕知道,退下吧", delegate { Debug.Log("clicked on yes"); });
                        SimplePopupManager.Instance.ShowPopup();
                    }
                }



            }
        }

        IEnumerator CreateResArgu(string itemType, int itemId, string attachmentId)
        {
            var url = string.Format(AppConfig.JiuquUrl+"/admin-course/item/attachment");
            WWWForm form = new WWWForm();
            form.AddField("itemType", itemType);
            form.AddField("itemId", itemId);
            form.AddField("name", ResName);
            form.AddField("attachmentId", attachmentId);
            UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
            webRequest.SetRequestHeader("Authorization", GetAuthCode());
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.LogWarning(": Error: " + webRequest.error);
            }
            else
            {
                Debug.LogWarning(webRequest.downloadHandler.text);
            }
        }
        IEnumerator UpdateArgu(string id, string itemType, string itemId, string name, string attachmentId)
        {
            var dict = new Dictionary<string, string>();
            dict["id"] = id;
            dict["itemType"] = itemType;
            dict["itemId"] = itemId;
            dict["name"] = name;
            dict["attachmentId"] = attachmentId;
            var argu = JsonConvert.SerializeObject(dict);
            var webRequest = UnityWebRequest.Put(AppConfig.JiuquUrl+"/admin-course/item/attachment/" + id, argu);
            webRequest.SetRequestHeader("Authorization", GetAuthCode());
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
            }
        }

        IEnumerator UploadZipToCloud(int itemId)
        {
            var template_id = _template.id;
            var filePath = _zipname;
            if (!File.Exists(filePath))
            {
                Debug.LogError("未发现资源！！！");
                yield return null;
            }
            else
            {
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length);
                fs.Close();
                WWWForm form = new WWWForm();
                form.AddBinaryData("ResConfig", bytes, "res.zip");
                var webRequest = UnityWebRequest.Post("http://stupideditor.yzqlwt.com/packages/uploadSingle", form);
                // var webRequest = UnityWebRequest.Post("http://localhost:3000/packages/uploadSingle", form);
                webRequest.SetRequestHeader("template-id", template_id+"");
                webRequest.SetRequestHeader("item-id", itemId+"");
                webRequest.SendWebRequest();
                var httpProgress = Instantiate(HttpProgress, transform);
                yield return StartCoroutine(httpProgress.GetComponent<WebRequest>().Process(webRequest, "正在备份到云..."));
                if (webRequest.isNetworkError)
                {
                    Debug.Log(": Error: " + webRequest.error);
                }
                else
                {
                    Debug.Log(webRequest.downloadHandler.text);
                }
            }
        }
        IEnumerator UploadZip(int itemId)
        {
            yield return StartCoroutine(UploadZipToCloud(itemId));
            var filePath = DirTools.GetBasePath()+"/res.zip";
            if (!File.Exists(filePath))
            {
                Debug.LogError("未发现资源！！！");
                yield return null;
            }
            else
            {
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length);
                fs.Close();
                WWWForm form = new WWWForm();
                form.AddBinaryData("file", bytes, "res.zip");
                var webRequest = UnityWebRequest.Post(AppConfig.JiuquUrl+"/admin-course/asset/uploadSingle", form);
                webRequest.SetRequestHeader("Authorization", GetAuthCode());
                webRequest.SendWebRequest();
                var httpProgress = Instantiate(HttpProgress, transform);
                yield return StartCoroutine(httpProgress.GetComponent<WebRequest>().Process(webRequest, "正在上传到后台..."));
                if (webRequest.isNetworkError)
                {
                    Debug.Log(": Error: " + webRequest.error);
                }
                else
                {
                    Debug.Log(webRequest.downloadHandler.text);
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(webRequest.downloadHandler.text);
                    var attachment_id = dict["id"];
                    _attachmentId = attachment_id;
                }
            }

        }

        IEnumerator DownloadZip(Attachment attachment)
        {
            if (attachment == null)
            {
                yield return null;
            }
            else
            {
                var url = string.Format("http://gate-static.97kid.com/{0}", attachment.attachments.uri);
                DateTime date = DateTime.Now;
                var dateStr = date.ToString("yyyyMMdd-HH时mm分ss秒");
                var zipPath = DirTools.GetDownloadDir()+"/"+dateStr+ "."+ attachment.attachments.ext_name;
                UnityWebRequest webRequest = UnityWebRequest.Get(url);
                var httpProgress = Instantiate(HttpProgress, transform);
                webRequest.SendWebRequest();
                yield return StartCoroutine(httpProgress.GetComponent<WebRequest>().Process(webRequest, "正在从后台下载..."));
                if (webRequest.isNetworkError)
                {
                    Debug.Log(": Error: " + webRequest.error);
                }
                else
                {
                    Debug.Log(zipPath);
                    var data = webRequest.downloadHandler.data;
                    File.WriteAllBytes(zipPath, data);
                    _zipname = zipPath;
                }
            }


        }

        public void EnsureClick()
        {
            var isOn = ScrollViewContent.GetComponent<ToggleGroup>().AnyTogglesOn();
            if (!isOn)
            {
                SimplePopup.SimplePopupManager.Instance.CreatePopup("必须要选择Skin");
                SimplePopup.SimplePopupManager.Instance.AddButton("知道了", delegate {  });
                SimplePopup.SimplePopupManager.Instance.ShowPopup();
            }
            else
            {
                var items = ScrollViewContent.GetComponent<ToggleGroup>().ActiveToggles();
                var item = items.FirstOrDefault();
                if (item)
                {
                    Debug.Log("GetAttachments");
                    var skinItemId = item.GetComponent<SkinItemScript>()._Config.id;
                    StartCoroutine(GetAttachments("Skin", skinItemId));
                    SkinsPanel.gameObject.SetActive(false);
                }

            }
        }
    }
    
}