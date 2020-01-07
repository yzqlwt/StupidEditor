using System.IO;

namespace StupidEditor
{
    using System.Collections;
    using System.Collections.Generic;
    using QFramework;
    using UnityEngine;
    using System.Linq;
    using Newtonsoft.Json;
    using QFramework.Example;
    public class GenerateJsonDone : ExportCommandDone
    {
        public List<string> Files;
    }
    public class GenerateJsonComponent : MonoBehaviour
    {
        void Start()
        {
        }
        public GenerateJsonDone SerializeToJson(List<ResourceInfo> TotalResInfo, List<string> plistName)
        {
            var count = 0;
            var dict = TotalResInfo.ToDictionary(key => key.FileName, value => 
            {
                var item = new ConfigItem()
                {
                    Name = value.FileName,
                    Time = value.Time,
                    Tag = ResourceTag.TagsMap[value.Tag],
                    Md5 = value.MD5,
                    Extension = value.Extension
                };
                if (item.Tag == ResourceTag.TagsMap[ResourceTag.TexturePackage])
                {
                    count = count + 1;
                }
                return item;
            });
            var json = JsonConvert.SerializeObject(new ConfigTemplate()
            {
                resource = dict,
                plist = count !=0 ? plistName : null
            }, Formatting.Indented);
            var path = DirTools.GetTempConfigPath() + "/ResConfig.json";
            File.WriteAllText(path, json);
            return new GenerateJsonDone()
            {
                Ret = true,
                Reason = "生成配置成功",
                Files = new List<string>()
                {
                    path
                }
            };
        }

    }
}
