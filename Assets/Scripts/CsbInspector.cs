using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
using QFramework;
using QFramework.Example;
using UIPanel = QFramework.Example.UIPanel;

namespace StupidEditor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class CsbInspectDone : ExportCommandDone
    {
        public List<string> Files;
    }
    
    public class CsbInspector : MonoBehaviour
    {
        public List<string> GetCsbImagePath(string path)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(path);
            var command = Application.streamingAssetsPath + "/CsbInspect/main.exe";
            var argu = path;
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
                return XMLAnalyzed(output);
            }
        }
        List<string> XMLAnalyzed(string xmlStrig)
        {
            XmlDocument xml = new XmlDocument();    //xml文件对象
            XmlReaderSettings set = new XmlReaderSettings();    //一个读取xml设置的对象
            set.IgnoreComments = true;  //设置忽略xml注释文档的影响，有时候注释会影响到xml的读取
            xml.LoadXml("<root> \n"+xmlStrig+" \n</root>");   //加载xml文件
            var list = xml.SelectNodes("/root/FileData");
            var paths = new List<string>();
            foreach (XmlNode node in list)
            {
                string path = node.Attributes["Path"].Value;
                paths.Add(path);
            }
            return paths;
        }
    }

}