using System.Collections;
using System.Collections.Generic;
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
    public Button DeleteBtn;
    public Toggle IsCsb;

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
                resourceItem.SetFileName(value + resourceItem.ResInfo.Extension);
            }
            else
            {
                Debug.LogError("mItem is null");
            }
        });
        IsCsb.onValueChanged.AddListener((ret) =>
        {
            if (mItem)
            {
                var resourceItem = mItem.GetComponent<ResourceItem>();
                resourceItem.SetIsCsb(ret);
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
    }

    void SetUI()
    {
        var ResInfo = mItem.GetComponent<ResourceItem>().ResInfo;
        InputName.text = ResInfo.FileName.Split('.')[0];
        ExtensionText.text = ResInfo.Extension;
        TimeText.text = ResInfo.Time.ToString();
        IsCsb.isOn = ResInfo.isCsb;
    }

}
