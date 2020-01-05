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
    public Dropdown Tags;

    private Toggle mItem;
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
                try
                {
                    resourceItem.SetFileName(value + resourceItem.ResInfo.Extension);
                }
                catch (IOException e)
                {
                    resourceItem.SetFileName(value + "的副本" +resourceItem.ResInfo.Extension);
                }
            }
            else
            {
                Debug.LogError("mItem is null");
            }
        });
        Tags.onValueChanged.AddListener((ret) =>
        {
            if (mItem)
            {
                var resourceItem = mItem.GetComponent<ResourceItem>();
                resourceItem.SetTag(Tags.captionText.text);
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
        Tags.ClearOptions();
        Tags.AddOptions(new List<string> {
           ResourceTag.TexturePackage,
           ResourceTag.None,
           ResourceTag.CocosStudio,
        });
    }

    void SetUI()
    {
        var ResInfo = mItem.GetComponent<ResourceItem>().ResInfo;
        InputName.text = ResInfo.FileName.Split('.')[0];
        ExtensionText.text = ResInfo.Extension;
        TimeText.text = "Import Time: "+ResInfo.Time.ToString("G");
        var index = Tags.options.FindIndex((option) => {
            return option.text == ResInfo.Tag;
        });
        Tags.SetValueWithoutNotify(index);    
        if(ResInfo.Width != 0 && ResInfo.Height != 0)
        {
            SizeText.gameObject.SetActive(true);
            SizeText.text = "Size: " + ResInfo.Width + "x" + ResInfo.Height;
        }
        else
        {
            SizeText.gameObject.SetActive(false);
        }
    }

}
