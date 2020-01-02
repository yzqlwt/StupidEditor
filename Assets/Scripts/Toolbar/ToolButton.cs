using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolButton : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Panel;
    void Start()
    {
       var toggle = transform.GetComponent<Toggle>();
       Panel.SetActive(toggle.isOn);
        toggle.onValueChanged.AddListener((isOn) => {
            Panel.SetActive(isOn);
        });
    }

}
