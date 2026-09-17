using seiko.framework.bases.exception;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Data;
using seiko.framework.gyomu.constant;

namespace seiko.framework.bases.utils
{
    public class PropertyXmlParser
    {
        // プロパティファイル要素名称（resources） 
        private static readonly string PROPERTY_ELEMENT = "resources";
        // プロパティファイル属性名称（key）
        private static readonly string PROPERTY_KEY = "key";
        // デフォルトタグ名称
        private static readonly string PROPERTY_DEFAULT_TAG = "item";
        // xmlドキュメント 
        private readonly XDocument xDocument;


        /// <summary>
        /// 業務プロパティXMLファイルを読み込む
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        public PropertyXmlParser(string fileName)
        {

            // ファイル名称チェック
            if (string.IsNullOrEmpty(fileName))
            {
                throw new GyomuException($"プロパティ読み込みエラー fileName:{fileName}");
            }

            // xmlファイルパス作成
            string filePath = FilePathUtil.SetStreamingAssetsPath(BusinessConstant.PROPERTY_FILE_PATH, fileName);
            if (!File.Exists(filePath))
            {
                throw new GyomuException($"プロパティ読み込みエラー filePath:{filePath}");

            }

            // xmlファイルから指定したデータを取得
            xDocument = XDocument.Load(filePath);

        }

        /// <summary>
        /// 指定されたKeyに対する値を返す
        /// </summary>
        /// <param name="grpName">グループ名</param>
        /// <param name="keyName">キー指定</param>
        /// <returns>取得結果</returns>
        public string GetItemValue(string grpName, string keyName)
        {
            return GetItemValue(grpName, PROPERTY_DEFAULT_TAG, keyName);
        }

        /// <summary>
        /// 指定されたKeyに対する値を返す
        /// </summary>
        /// <param name="grpName">グループ名</param>
        /// <param name="tagName">タグ名称</param>
        /// <param name="keyName">キー指定</param>
        /// <returns>取得結果</returns>
        public string GetItemValue(string grpName, string tagName, string keyName)
        {
            string xelementValue = null;

            XElement xelement = xDocument.Element(PROPERTY_ELEMENT).Element(grpName)
                .Elements(tagName).Where(x => x.Attribute(PROPERTY_KEY).Value.Equals(keyName)).FirstOrDefault();

            if (xelement != null)
            {
                // 値が取得できた場合は戻り値にセット
                xelementValue = xelement.Value;
            }

            return xelementValue;
        }

    }
}
