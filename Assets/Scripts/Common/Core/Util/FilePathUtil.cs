// using Cysharp.Threading.Tasks;
using seiko.framework.bases.exception;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace seiko.framework.bases.utils
{
    /// <summary>
    /// ファイルパス操作クラス
    /// </summary>
    public static class FilePathUtil
    {

        /// <summary>
        /// ファイルパスの設定
        /// </summary>
        /// <param name="baseFolderPath">フォルダパス</param>
        /// <param name="fileName">ファイル名</param>
        /// <returns>環境ごとのファイルパス</returns>
        public static string SetPersistentDataPathPath(string baseFolderPath, string fileName)
        {
            return Path.Combine(Application.persistentDataPath, baseFolderPath, fileName);
        }


        /// <summary>
        /// StreamingAssets内のファイルパスの設定
        /// </summary>
        /// <param name="baseFolderPath">フォルダパス</param>
        /// <param name="fileName">ファイル名</param>
        /// <returns>外部ファイルのパス</returns>
        public static string SetStreamingAssetsPath(string baseFolderPath, string fileName)
        {
            return Path.Combine(Application.streamingAssetsPath, baseFolderPath, fileName);
        }


        /// <summary>
        /// 出力ファイルパスの設定
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <param name="baseFilePath">出力先のフォルダパス</param>
        /// <returns>出力ファイルパス</returns>
        public static async Task<string> SetOutputFilePathAsync(string fileName, string baseFilePath)
        {
            await Task.Run(() => { });
            string filePath = "";
            try
            {
                // filePath = SetPersistentDataPathPath(fileName);
                filePath = SetPersistentDataPathPath(baseFilePath, fileName);
            }
            catch (Exception e)
            {
                throw new SystemErrorException("ファイルパス取得エラー", e);
            }

            return filePath;
        }

    }
}
