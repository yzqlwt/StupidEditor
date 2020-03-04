using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.IO;

public class TestFtp : MonoBehaviour
{
    public string FTPHost = "ftp://39.104.112.72:2121/";
    public string FTPUserName = "resconfig";
    public string FTPPassword = "resconfig";
    public string FilePath;

    public void UploadFile()
    {
        FilePath = Application.dataPath + "/StreamingAssets/texture/cocos.png";
        Debug.Log("Path: " + FilePath);


        WebClient client = new System.Net.WebClient();
        Uri uri = new Uri(FTPHost + new FileInfo(FilePath).Name);

        client.UploadProgressChanged += new UploadProgressChangedEventHandler(OnFileUploadProgressChanged);
        client.UploadFileCompleted += new UploadFileCompletedEventHandler(OnFileUploadCompleted);
        client.Credentials = new System.Net.NetworkCredential(FTPUserName, FTPPassword);
        client.UploadFileAsync(uri, "STOR", FilePath);
    }

    void OnFileUploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
    {
        Debug.Log("Uploading Progreess: " + e.ProgressPercentage);
    }

    void OnFileUploadCompleted(object sender, UploadFileCompletedEventArgs e)
    {
        Debug.Log("File Uploaded");
    }

    void Start()
    {
        UploadFile();
    }
}