using System.Collections.Generic;
using UnityEngine;


namespace Framework
{
    class UIPanelInfos
    {
        public List<UIPanelInfo> panelInfoList;
    }

    [System.Serializable]
    public class UIPanelInfo : ISerializationCallbackReceiver
    {

        [System.NonSerialized]
        public PanelType panelType;

        [System.NonSerialized]
        public UILevel level;

        public string panelTypeString; // Json解析赋值给string，不能给Enum
        public string path;
        public string levelString;


        public void OnAfterDeserialize()
        {
            // 反序列化之后调用，即从Json文本数据解析至对象之后，会进行调用
            panelType = (PanelType)System.Enum.Parse(typeof(PanelType), panelTypeString.ToUpper());
            level = (UILevel)System.Enum.Parse(typeof(UILevel), levelString);
        }

        public void OnBeforeSerialize()
        {
        }
    }
}


