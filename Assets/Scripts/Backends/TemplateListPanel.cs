using System;
using System.Linq;
using Newtonsoft.Json;
using QFramework;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace StupidEditor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    public class Template
    {
        public int id;
        public string name;
        public string game_id;
    }
    class TemplateList
    {
        public int start;
        public int count;
        public int total;
        public List<Template> targets;
    }

    public class Preview
    {
        // "preview": {
        //     "id": 7018,
        //     "name": "{42D94314-767B-4291-828E-4C960BBF6041}_20200107131946.jpg",
        //     "uri": "/course-math/5a/f4/5a7b495b1106fe8aacb47e01e0f11bf4.jpg",
        //     "ext_name": "jpg"
        // }
        public int id;
        public string name;
        public string uri;
        public string ext_name;
    }

    public class SkinConfig
    {
        public int id;
        public string name;
        public string skin_id;
        public Preview preview;
    }
    public class TemplateListPanel : MonoBehaviour
    {

        public Dropdown TemplateList;
        public Transform ScrollViewContent;
        public Transform SkinItem;
        private Dictionary<string, Template> templates;
        private bool _active;
        public bool Active
        {
            get { return _active; }
            set
            {
                Debug.Log("BackendCliSetActiveSetActiveSetActiveSetActive");
                Debug.Log(value);
                _active = value;
                transform.gameObject.SetActive(value);
                if (value)
                {
                    StartCoroutine(GetTemplates());
                }
            }
        }
        

        void Start()
        {
            TemplateList.onValueChanged.AddListener((value) =>
            {
                var text = TemplateList.captionText.text;
                var template = templates[text];
                TypeEventSystem.Send(template);
                transform.gameObject.SetActive(false);
            });
            
        }

        public string GetAuthCode()
        {
            var token = JsonConvert.DeserializeObject<AccessTokenStruct>(PlayerPrefs.GetString("token"));
            return token.access_token;
        }
        public void GetListClick()
        {
            StartCoroutine(GetTemplates());
        }

        public void BtnCloseClick()
        {
            transform.gameObject.SetActive(false);
        }
        IEnumerator GetTemplates()
        {
            TemplateList.GetComponent<Dropdown>().ClearOptions();
            var start = 0;
            var uri = AppConfig.JiuquUrl+"/admin-course/activityTemplates?count=100&start=" + start;
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                webRequest.SetRequestHeader("Authorization", GetAuthCode());
                webRequest.SetRequestHeader("Content-Type", "application/json");
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError)
                {
                    Debug.Log(": Error: " + webRequest.error);
                }
                else
                {
                    var templateList = JsonConvert.DeserializeObject<TemplateList>(webRequest.downloadHandler.text);
                    var activities = templateList.targets.Select((temp) => { return temp.game_id+"<=>"+temp.name; }).ToList();
                    activities.Insert(0, "未选择Template");
                    TemplateList.GetComponent<Dropdown>().AddOptions(activities);
                    templates = templateList.targets.ToDictionary((key) => { return key.game_id+"<=>"+key.name; }, value=>value);
                }
            }
        }
        
    }

}