

using System;
using System.Collections;

namespace StupidEditor
{
    using UnityEngine;
    using UnityEngine.UI;
    public class SkinItemScript: MonoBehaviour
    {
        public Image PreviewImage;
        public Text SkinName;
        public Text SkinId;


        void Start()
        {
            
        }

        public void SetSkin(SkinConfig config)
        {
            SkinName.text = "SkinName: "+config.name;
            SkinId.text = "SkinId: "+config.skin_id;
            StartCoroutine(GetImage("http://gate-static.97kid.com/" + config.preview.uri));
        } 
        IEnumerator GetImage(string uri)
        {
            var size = PreviewImage.GetComponent<RectTransform>().sizeDelta;
            var whRatio = size.x / size.y;
            var path =  uri;
#pragma warning disable CS0618 // 类型或成员已过时
            var www = new WWW(path);
#pragma warning restore CS0618 // 类型或成员已过时
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                Texture2D tex = www.texture;
                if(tex.width/tex.height > whRatio)
                {
                    var y = size.x / tex.width * tex.height;
                    PreviewImage.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, y);
                }
                else
                {
                    var x = size.y / tex.height * tex.width;
                    PreviewImage.GetComponent<RectTransform>().sizeDelta = new Vector2(x, size.y);
                }
                Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
                PreviewImage.sprite = temp;
            }

        }
    }
}