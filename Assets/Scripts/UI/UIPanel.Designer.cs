//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QFramework.Example
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;
    
    
    public partial class UIPanel
    {
        
        public const string NAME = "UIPanel";
        
        private UIPanelData mPrivateData = null;
        
        public UIPanelData mData
        {
            get
            {
                return mPrivateData ?? (mPrivateData = new UIPanelData());
            }
            set
            {
                mUIData = value;
                mPrivateData = value;
            }
        }
        
        protected override void ClearUIComponents()
        {
            mData = null;
        }
    }
}
