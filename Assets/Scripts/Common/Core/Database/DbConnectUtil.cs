using Mono.Data.Sqlite;
using Scripts.Common.Features.Config;
using seiko.framework.bases.exception;
using seiko.framework.bases.utils;
using System;
using UnityEngine;

namespace seiko.framework.bases.db
{
    /// <summary>
    /// DB接続クラス
    /// </summary>
    public static class DbConnectUtil
    {
        // 設定情報
        static DBConfig _dbConfig;

        // コネクションインスタンス
        public static SqliteConnection Connection;

        // 再試行回数
        static int _count = 0;

        // 再試行回数上限
        static int _retryTimes;

        public static void ConnectDB(DBConfig config)
        {
            _dbConfig = config;
            _retryTimes = _dbConfig.ReteryTimes;
            try
            {

                string dbPath = FilePathUtil.SetPersistentDataPathPath(_dbConfig.FilePath, _dbConfig.FileName);

                Debug.Log("dbPath: " + dbPath);

                Connection = new SqliteConnection($"URI=file:{dbPath}");
                // SQLiteの設定 //今後どのような挙動なのか要確認
                // SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
                Connection.Open();
                _count = 0;

                if (Connection.State == System.Data.ConnectionState.Open)
                {
                    Debug.Log($"{_dbConfig.FileName}_データベース接続成功: " + Connection.State);
                }
                else
                {
                    Debug.LogError("データベース接続に失敗しました: " + Connection.State);
                }
            }
            catch (Exception e)
            {
                if (_count < _retryTimes)
                {
                    _count++;
                    ConnectDB(config);
                }
                else
                {
                    _count = 0;
                    throw new SystemErrorException("DB接続：エラー", e);
                }
            }
        }

        /// <summary>
        /// コネクション切断
        /// </summary>
        public static void DisconnectDB()
        {
            try
            {
                Connection.Close();
                _count = 0;
            }
            catch (Exception e)
            {
                if (_count < _retryTimes)
                {
                    _count++;
                    DisconnectDB();
                }
                else
                {
                    _count = 0;
                    throw new SystemErrorException("DB切断：エラー", e);
                }
            }
        }
    }
}