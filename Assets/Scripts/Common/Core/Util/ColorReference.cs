using UnityEngine;
using seiko.framework.gyomu.constant;
using seiko.framework.bases.utils;

namespace seiko.UI
{
    public static class ColorReference
    {

        public static Color SosaStatusColor(int status)
        {
            // プロパティから処理名称を取得
            string keyName = "sosaStatus" + status.ToString();
            string targetColor = GetPropItemValue(BusinessConstant.PROP_GROUP_APP_COLOR, keyName);

            return HexToColor(targetColor);
        }

        public static Color SosaStepColor(int status)
        {
            // プロパティから処理名称を取得
            string keyName = "sosaStep" + status.ToString();
            string targetColor = GetPropItemValue(BusinessConstant.PROP_GROUP_APP_COLOR, keyName);

            return HexToColor(targetColor);
        }

        public static Color HexToColor(string hex)
        {
            // HEXカラーコードからColorに変換
            if (ColorUtility.TryParseHtmlString(hex, out Color color))
            {
                return color;
            }

            // カラーコードが存在しない場合は白を返却
            return Color.white;
        }

        private static string GetPropItemValue(string grpName, string keyName)
        {
            // プロパティXMLファイルをロード
            PropertyXmlParser propXml = new(BusinessConstant.CMN_PROPERTY_FILE_NAME);

            // プロパティから値を取得
            return propXml.GetItemValue(grpName, keyName);
        }
    }
}


