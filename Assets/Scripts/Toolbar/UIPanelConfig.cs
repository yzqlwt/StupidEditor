using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEngine.UI;
public class UIPanelConfig : MonoSingleton<UIPanelConfig>
{
    public Toggle toggleAltas;
    public bool IsAltas {
        get
        {
            return toggleAltas.isOn;
        }
        set
        {
            toggleAltas.isOn = value;
        }
    }
    private void Start()
    {
        IsAltas = false;
    }
}
