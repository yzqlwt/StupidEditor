using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WebRequest : MonoBehaviour {

    public Slider slider;
    public Text text;//内容显示
    public Text progressText;//进度显示

    void Start()
    {
        // var url = "http://gate-static.97kid.com//course-math/f6/d5/f6b81ae8f611784c48225629c18c76d5.zip";
        // var path = @"C:\Users\yzqlwt\Desktop/StupidEditorData/Temp/download/20200115-15时19分46秒.zip";
        // transform.gameObject.SetActive(false);
        // StartCoroutine(StartRequest(url, path));
    }

    public IEnumerator StartRequest(string uri, string path)
    {
        transform.gameObject.SetActive(true);
        yield return StartCoroutine(Download(uri, path));
    }

    //http://gate-static.97kid.com//course-math/f6/d5/f6b81ae8f611784c48225629c18c76d5.zip
    //C:\Users\yzqlwt\Desktop/StupidEditorData/Temp/download/20200115-15时19分46秒.zip
    IEnumerator Download(string uri, string path)
    {
        using (UnityWebRequest uwr = UnityWebRequest.Get(uri))
        {
            uwr.SendWebRequest();//开始请求
            while (!uwr.isDone)
            {
                //Debug.LogError(www.downloadProgress);
                slider.value = uwr.downloadProgress;//展示下载进度
                progressText.text = Math.Floor(uwr.downloadProgress * 100) + "%";
                Debug.Log(progressText.text);
                yield return 1;
            }
            if (uwr.isDone)
            {
                progressText.text = 100 + "%";
                slider.value = 1;
            }
            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                text.text = "ok!";
                byte[] results = uwr.downloadHandler.data;
                var data = uwr.downloadHandler.data;
                File.WriteAllBytes(path, data);
                Destroy(gameObject);
            }
        }
    }
    public IEnumerator Process(UnityWebRequest uwr, string desc = "传输中。。。")
    {
        text.text = desc;
        while (!uwr.isDone)
        {
            //Debug.LogError(www.downloadProgress);
            slider.value = uwr.downloadProgress;//展示下载进度
            progressText.text = Math.Floor(uwr.downloadProgress * 100) + "%";
            Debug.Log(progressText.text);
            yield return 1;
        }
        if (uwr.isDone)
        {
            progressText.text = 100 + "%";
            slider.value = 1;
        }
        if (uwr.isNetworkError || uwr.isHttpError)
        {
            Debug.Log(uwr.error);
        }
        else
        {
            text.text = "ok!";
            Destroy(gameObject);
        }
    }
}
