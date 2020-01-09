namespace StupidEditor
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Linq;
    using B83.Win32;
    using System.Threading.Tasks;
    using System.Collections;
    using System.IO;
    using QFramework;
    
    
    public class FileDragAndDrop : MonoBehaviour
    {
        // important to keep the instance alive while the hook is active.
        UnityDragAndDropHook hook;
        void OnEnable ()
        {
            // must be created on the main thread to get the right thread id.
            hook = new UnityDragAndDropHook();
            hook.InstallHook();
            hook.OnDroppedFiles += OnFilesAsync;
        }
    
        void OnDisable()
        {
            hook.UninstallHook();
        }
    
        void OnFilesAsync(List<string> aFiles, POINT aPos)
        {
            var existCsb = aFiles.ToList<string>().Find((file) => file.EndsWith(".csb")) != null;
            foreach(var path in aFiles)
            {
                if (Directory.Exists(path))
                {
                    var files = Directory.GetFiles(path, "*");
                    var isCsb = files.ToList<string>().Find((file) => file.EndsWith(".csb")) != null;
                    Directory.GetFiles(path, "*").ForEach((file) => {
                        TypeEventSystem.Send(new FileDragIn()
                        {
                            Path = file,
                            Tag  = isCsb ? ResourceTag.CocosStudio : ResourceTag.Default,
                            Point = aPos
                        });
                    });
                }else if (File.Exists(path))
                {
                    TypeEventSystem.Send(new FileDragIn()
                    {
                        Path = path,
                        Tag = existCsb ? ResourceTag.CocosStudio : ResourceTag.Default,
                        Point = aPos
                    });
                }
            
            }
        }
    }

}