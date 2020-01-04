namespace StupidEditor
{
    using System.Collections;
    using System.Collections.Generic;
    using QFramework;
    using UnityEngine;
    using System.Linq;
    using Newtonsoft.Json;
    using QFramework.Example;

    public class GenerateJson : MonoBehaviour
    {
        private List<ResourceInfo> TotalResInfo;
        void Start()
        {
            TypeEventSystem.Register<ExportCommand>((tex) => {
                if(tex.command == SequenceCommand.GenerateJson)
                {
                    TotalResInfo = tex.TotalResInfo;
                    SerializeToJson();
                }
            });
        }
        void SerializeToJson()
        {
            var dict = TotalResInfo.ToDictionary(key => key.FileName, value => 
            {
                var item = new ConfigItem()
                {
                    Name = value.FileName,
                    Time = value.Time,
                    Tag = ResourceTag.TagsMap[value.Tag]
                };
                return item;
            });
            var json = JsonConvert.SerializeObject(new ConfigTemplate()
            {
                resource = dict
            }, Formatting.Indented);
            Debug.Log(json);
            TypeEventSystem.Send(new ExportCommandDone()
            {
                Ret = true,
                Reason = "json生成完成"
            });
        }

    }
}
