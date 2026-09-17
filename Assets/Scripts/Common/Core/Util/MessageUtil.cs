using Newtonsoft.Json.Linq;
using seiko.framework.bases.exception;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using seiko.framework.gyomu.constant;
using UnityEngine;

namespace seiko.framework.bases.utils
{
    /// <summary>
    /// メッセージを取得するUtilクラス
    /// </summary>
    public class MessageUtil
    {
        // メッセージ格納変数
        private static IEnumerable<JObject> messageJson;

        /// <summary>
        /// メッセージファイル取り込み
        /// </summary>
        public static void InstallMessageFile()
        {
            try
            {
                // メッセージファイル格納パス
                string messageFilePath = FilePathUtil
                    .SetStreamingAssetsPath(BusinessConstant.JSON_FILE_PATH, BusinessConstant.MESSAGE_DATA_FILE_NAME);

                //JSONファイルの読み込み
                using (StreamReader reader = new StreamReader(messageFilePath, Encoding.GetEncoding("UTF-8")))
                {
                    JArray jsonArray = JArray.Parse(reader.ReadToEnd());
                    messageJson = jsonArray.Cast<JObject>();
                }
            }
            catch (Exception e)
            {
                throw new SystemErrorException("メッセージファイル取得：エラー", e);
            }
        }

        /// <summary>
        /// メッセージの取得
        /// </summary>
        /// <param name="messageKey">メッセージキー</param>
        /// <param name="bindParameter">バインドパラメータリスト</param>
        /// <returns>取得結果</returns>
        public static Dictionary<string, string> GetMessageData(string messageKey, List<string> msgBindParameter, List<string> logBindParameter)
        {
            try
            {
                string level = null;
                string message = null;
                string logMsg = null;

                // メッセージキーからデータを取得
                var result = messageJson.FirstOrDefault(obj => obj["key"].ToString() == messageKey);
                if (result != null)
                {
                    level = result["level"].ToString();
                    message = result["msg"].ToString();
                    logMsg = result["logMsg"].ToString();

                    // バインドパラメータがある場合は、メッセージへ書き込む
                    if (msgBindParameter != null && msgBindParameter.Count > 0)
                    {
                        for (int i = 0; i < msgBindParameter.Count; i++)
                        {
                            message = message.Replace("[%" + (i + 1) + "]", msgBindParameter[i]);
                        }
                    }
                    if (logBindParameter != null && logBindParameter.Count > 0)
                    {
                        for (int i = 0; i < logBindParameter.Count; i++)
                        {
                            logMsg = logMsg.Replace("[%" + (i + 1) + "]", logBindParameter[i]);
                        }
                    }
                }

                return new Dictionary<string, string>()
                    {
                        { "level",level},
                        { "msg", message},
                        { "logMsg", logMsg}
                    };

            }
            catch (Exception e)
            {
                throw new SystemErrorException("メッセージ取得：エラー", e);
            }

        }

    }
}
