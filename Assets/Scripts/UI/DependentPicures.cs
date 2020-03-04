using System.IO;
using System.Linq;
using DebounceThrottle;
using QFramework;
using UIPanel = QFramework.Example.UIPanel;

namespace StupidEditor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class DependentPicures : MonoBehaviour
    {
        // Start is called before the first frame update

        private DebounceDispatcher debounceDispatcher;
        private List<string> Paths;
        void Start()
        {
            debounceDispatcher = new DebounceDispatcher(1000);
            TypeEventSystem.Register<FileDragIn>((fileDragIn) =>
            {
                var path = fileDragIn.Path;
                System.IO.FileInfo file = new System.IO.FileInfo(path);
                if (file.Extension == ".csb")
                {
                    Paths = GetPaths();
                }else if (file.Extension == ".png")
                {
                    RefreshItems();
                }
            });
            TypeEventSystem.Register<DeleteItem>((t) =>
            {
                if (t.Extension == ".csb")
                {
                    Paths = GetPaths();
                    RefreshItems();
                }
            });
        }

        public List<string> GetPaths(string path="")
        {

            var csbInspector = transform.GetComponent<CsbInspector>();
            var totalInfo = transform.GetComponent<QFramework.Example.UIPanel>().GetTotalInfo();
            totalInfo = totalInfo.Where((info) => { return info.Extension == ".csb"; }).ToList();
            var paths = totalInfo.Select((info) => { return info.FileFullName; }).ToList();
            paths = paths.Distinct().ToList();
            var totalPaths = new List<string>();
            paths.ForEach((name) =>
            {
                Debug.LogWarning(name);
                totalPaths.AddRange(csbInspector.GetCsbImagePath(name));
            });
            totalPaths = totalPaths.Select((t) => { return Path.GetFileName(t); }).ToList();
            totalPaths = totalPaths.Distinct().ToList();
            return totalPaths;
        }

        public void RefreshItems()
        {
            debounceDispatcher.Debounce(() =>
            {
                var totalPaths = Paths;
                totalPaths.ForEach((i) =>
                {
                    Debug.Log("ddddd+++++"+i);
                });
                var totalItems = GetTotalItem();
                totalItems.ForEach((item) =>
                {
                    var resItem = item.GetComponent<ResourceItem>();
                    var resInfo = resItem.ResInfo;
                    resInfo.Tag = totalPaths.Contains(resInfo.FileName) ? ResourceTag.CocosStudio : ResourceTag.Default;
                    resItem.SetUI();
                });
            });
        }
        
        public List<Transform> GetTotalItem()
        {
            return transform.GetComponent<UIPanel>().GetTotalItem();
        }
        
    }

}