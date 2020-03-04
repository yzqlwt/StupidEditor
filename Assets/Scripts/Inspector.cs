using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QFramework;
using StupidEditor;
using UnityEngine;
using UnityEngine.UI;

public class ResourceItemClick
{

}

public class Inspector : MonoBehaviour
{
    // Start is called before the first frame update
    public ToggleGroup ScrollViewContent;
    public GameObject Mask;
    public InputField InputName;
    public Text ExtensionText;
    public Text TimeText;
    public Text SizeText;
    public Button DeleteBtn;
    public Toggle ImageMark;

    private Toggle mItem;
    public Transform SelectItem { 
        get {return mItem.transform;}
    }
    void Start()
    {
        Mask.SetActive(true);
        TypeEventSystem.Register<ResourceItemClick>((tmp)=> {
            var isOn = ScrollViewContent.AnyTogglesOn();
            if (!isOn)
            {
                Mask.SetActive(true);
            }
            else
            {
                Mask.SetActive(false);
                var items = ScrollViewContent.ActiveToggles();
                var item = items.FirstOrDefault();
                if (item)
                {
                    mItem = item;
                    SetUI();
                }

            }
        });
        InputName.onValueChanged.AddListener((value) =>{
            if (mItem)
            {
                var resourceItem = mItem.GetComponent<ResourceItem>();
                resourceItem.SetFileName(value + resourceItem.ResInfo.Extension);
            }
            else
            {
                Debug.LogError("mItem is null");
            }
        });

        DeleteBtn.onClick.AddListener(() =>
        {
            if (mItem)
            {
                var resourceItem = mItem.GetComponent<ResourceItem>();
                resourceItem.DeleteItem();
                Mask.SetActive(true);
            }
            else
            {
                Debug.LogError("mItem is null");
            }
        });
        ImageMark.onValueChanged.AddListener((state) =>
        {
            if (mItem)
            {
                var resInfo = mItem.GetComponent<ResourceItem>().ResInfo;
                var isCocosStudio = resInfo.Tag == ResourceTag.CocosStudio;
                var resourceItem = mItem.GetComponent<ResourceItem>();
                
                InputName.textComponent.color = isCocosStudio ? new Color(255, 0, 0) : new Color(0, 0, 0) ;
                if (state)
                {
                    resInfo.Tag = ResourceTag.TexturePackage;
                    resourceItem.SetTag(ResourceTag.TexturePackage);
                }
                else
                {
                    resInfo.Tag = ResourceTag.None;
                    resourceItem.SetTag(ResourceTag.None);
                }
            }
            else
            {
                Debug.LogError("mItem is null");
            }
        });

    }

    void SetUI()
    {
        var resInfo = mItem.GetComponent<ResourceItem>().ResInfo;
        InputName.SetTextWithoutNotify(resInfo.FileName.Split('.')[0]);
        ExtensionText.text = resInfo.Extension;
        TimeText.text = "Import Time: "+resInfo.Time.ToString("G");
        SizeText.gameObject.SetActive(resInfo.Width != 0 && resInfo.Height != 0);
        SizeText.text = "Size: " + resInfo.Width + "x" + resInfo.Height;
        var isCocosStudio = resInfo.Tag == ResourceTag.CocosStudio;
        InputName.readOnly = isCocosStudio;
        InputName.textComponent.color = isCocosStudio ? new Color(255, 0, 0) : new Color(0, 0, 0) ;
        ImageMark.gameObject.SetActive(!isCocosStudio);
        ImageMark.SetIsOnWithoutNotify(resInfo.Tag==ResourceTag.TexturePackage);
    }

}
