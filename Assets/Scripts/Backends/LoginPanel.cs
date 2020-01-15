using System;
using Newtonsoft.Json;

namespace StupidEditor
{
    using System.Collections;
    using System.Collections.Generic;
    using QFramework;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.UI;

    public class AppConfig
    {
        public static string JiuquUrl = "https://gate2.betamath.com";
    }
    public class AccessTokenStruct
    {
        public string access_token;
        public string token_type;
        public string refresh_token;
        public long expires_in;
    }
    
    public class LoginPanel : MonoBehaviour
    {
        public InputField Mobile;
    
        public InputField Passwd;
    
        public InputField Google;
        
        // Start is called before the first frame update


        void Start()
        {
            if (PlayerPrefs.HasKey("token"))
            {
                var token = JsonConvert.DeserializeObject<AccessTokenStruct>(PlayerPrefs.GetString("token"));
                var now = GetTimeStamp();
                if(token.expires_in < now + 1000 * 60 * 5)
                {
                    StartCoroutine(RefreshToken(token.refresh_token));
                }
                else
                {
                    transform.gameObject.SetActive(false);
                }
            }
            else
            {
                transform.gameObject.SetActive(true);
            }
            
        }

        public void LoginClick()
        {
            Debug.Log("LoginClick");
            StartCoroutine(Login());
        }
    
        IEnumerator  Login()
        {
            Debug.Log("Login");
            var mobile = Mobile.text;
            var google = Google.text;
            var passwd = Passwd.text;
            WWWForm form = new WWWForm();
            form.AddField("mobile",mobile);
            form.AddField("nonce", google);
            form.AddField("password", passwd);
            var webRequest = UnityWebRequest.Post(AppConfig.JiuquUrl+"/a/user/login", form);
            //webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                SimplePopup.SimplePopupManager.Instance.CreatePopup("登录失败");
                SimplePopup.SimplePopupManager.Instance.AddButton("知道了", delegate {  });
                SimplePopup.SimplePopupManager.Instance.ShowPopup();
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                var token = JsonConvert.DeserializeObject<AccessTokenStruct>(webRequest.downloadHandler.text);
                // var token = QF.SerializeHelper.FromJson<AccessToken>(webRequest.downloadHandler.text);
                if(token.access_token != null)
                {
                    StoreToken(token);
                    transform.gameObject.SetActive(false);
                }
                else
                {
                    SimplePopup.SimplePopupManager.Instance.CreatePopup("登录失败");
                    SimplePopup.SimplePopupManager.Instance.AddButton("知道了", delegate {  });
                    SimplePopup.SimplePopupManager.Instance.ShowPopup();
                }
            }
        }
        
        IEnumerator RefreshToken(string refresh_token)
        {
            var url = string.Format(AppConfig.JiuquUrl + "/a/user/refresh");
            WWWForm form = new WWWForm();
            form.AddField("refresh_token", refresh_token);
            var webRequest = UnityWebRequest.Post(url, form);
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                SimplePopup.SimplePopupManager.Instance.CreatePopup("登录失败");
                SimplePopup.SimplePopupManager.Instance.AddButton("知道了", delegate {  });
                SimplePopup.SimplePopupManager.Instance.ShowPopup();
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                var token = JsonConvert.DeserializeObject<AccessTokenStruct>(webRequest.downloadHandler.text);
                if (token.access_token != null)
                {
                    StoreToken(token);
                    transform.gameObject.SetActive(false);
                }
                else
                {
                    Debug.LogError(webRequest.downloadHandler.text);
                }
            }
        }
        
        private void StoreToken(AccessTokenStruct token)
        {
            var expires_in = token.expires_in * 1000 + GetTimeStamp();
            token.expires_in = expires_in;
            var str = JsonConvert.SerializeObject(token);
            PlayerPrefs.SetString("token", str);
        }
        
        private long GetTimeStamp()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }
        
    }

}