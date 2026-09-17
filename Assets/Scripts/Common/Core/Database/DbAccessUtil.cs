using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Mono.Data.Sqlite;
using System.Data;
using seiko.framework.bases.exception;
using seiko.framework.bases.utils;

namespace seiko.framework.bases.db
{
    /// <summary>
    /// DBアクセスクラス
    /// </summary>
    public static class DbAccessUtil
    {

        // SQLファイル格納フォルダパス
        private static readonly string sqlPath = "file/sql";

        /// <summary>
        /// SQL取得
        /// </summary>
        /// <param name="fileName">SQLファイル名</param>
        /// <param name="sqlId">SQLのID</param>
        /// <param name="sqlType">SQLのタイプ</param>
        /// <returns>SQL</returns>
        public static string GetSql(string fileName, string sqlId, SqlType sqlType)
        {
            try
            {
                // SQLファイルパス作成
                string filePath = FilePathUtil.SetStreamingAssetsPath(sqlPath, fileName);

                // SQLファイルから指定したSQLを取得
                XDocument xml = XDocument.Load(filePath);
                XElement xelement = xml.Element("sql").Elements(GetSqlType(sqlType)).Where(x => x.Attribute("id").Value.Equals(sqlId)).First();

                // 改行などを削除
                string sql = xelement.Value;
                sql = sql.Replace("\r", " ").Replace("\n", " ").Trim();

                return sql;
            }
            catch (Exception e)
            {
                throw new SystemErrorException("SQL取得：エラー", e);
            }
        }

        /// <summary>
        /// SQLタイプ(文字)の取得
        /// </summary>
        /// <param name="sqlType">SQLタイプ(ENUM)</param>
        /// <returns>SQLタイプ(文字)</returns>
        private static string GetSqlType(SqlType sqlType)
        {
            return sqlType.ToString().ToLower();
        }

        /// <summary>
        /// 検索系SQL処理
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="bindParameter">バインドパラメータ</param>
        /// <param name="connection">DBコネクション</param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> ExecuteReader(string sql, List<object> bindParameter, SqliteConnection connection)
        {
            try
            {
                List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

                using (SqliteCommand cmd = connection.CreateCommand())
                {

                    // SQL設定
                    cmd.CommandText = sql;

                    SetBindParameter(cmd, bindParameter);

                    // select実行
                    using (SqliteDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read() == true)
                        {
                            Dictionary<string, object> map = new Dictionary<string, object>();

                            for (int i = 0; i < sdr.FieldCount; i++)
                            {
                                map.Add(sdr.GetName(i), sdr[sdr.GetName(i)]);
                            }

                            result.Add(map);
                        }
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw new SystemErrorException("DBアクセス：エラー", e);
            }

        }

        /// <summary>
        /// 更新系SQL処理
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="bindParameter">バインドパラメータ</param>
        /// <param name="connection">DBコネクション</param>
        /// <returns></returns>
        public static int ExecuteWriter(string sql, List<object> bindParameter, SqliteConnection connection)
        {
            return ExecuteWriter(sql, bindParameter, connection, null);
        }

        /// <summary>
        /// 更新系SQL処理
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="bindParameter">バインドパラメータ</param>
        /// <param name="connection">DBコネクション</param>
        /// <param name="transaction">DBトランザクション</param>
        /// <returns></returns>
        public static int ExecuteWriter(string sql, List<object> bindParameter, SqliteConnection connection, SqliteTransaction transaction)
        {
            try
            {
                using (SqliteCommand cmd = connection.CreateCommand())
                {
                    // SQL設定
                    cmd.CommandText = sql;
                    cmd.Transaction = transaction;

                    SetBindParameter(cmd, bindParameter);

                    // SQLの実行
                    return cmd.ExecuteNonQuery();

                }
            }
            catch (Exception e)
            {
                throw new SystemErrorException("DBアクセス：エラー", e);
            }

        }

        /// <summary>
        /// バインドパラメータの設定
        /// </summary>
        /// <param name="cmd">コマンド</param>
        /// <param name="bindParameter">バインドパラメータ</param>
        private static void SetBindParameter(SqliteCommand cmd, List<object> bindParameter)
        {
            if (bindParameter != null && bindParameter.Count > 0)
            {
                for (int i = 0; i < bindParameter.Count; i++)
                {
                    if (bindParameter[i] == null || bindParameter[i] == DBNull.Value)
                    {
                        cmd.Parameters.Add("param" + (i + 1), DbType.Object);
                        cmd.Parameters["param" + (i + 1)].Value = DBNull.Value;
                    }
                    // 文字列
                    else if (bindParameter[i] is string)
                    {
                        cmd.Parameters.Add("param" + (i + 1), DbType.String);
                        cmd.Parameters["param" + (i + 1)].Value = bindParameter[i];
                    }
                    // 数値(整数)
                    else if (bindParameter[i] is int)
                    {
                        cmd.Parameters.Add("param" + (i + 1), DbType.Int32);
                        cmd.Parameters["param" + (i + 1)].Value = bindParameter[i];
                    }
                    else if (bindParameter[i] is long)
                    {
                        cmd.Parameters.Add("param" + (i + 1), DbType.Int64);
                        cmd.Parameters["param" + (i + 1)].Value = bindParameter[i];
                    }
                    // 数値(小数点)
                    else if (bindParameter[i] is double)
                    {
                        cmd.Parameters.Add("param" + (i + 1), DbType.Double);
                        cmd.Parameters["param" + (i + 1)].Value = bindParameter[i];
                    }
                    else if (bindParameter[i] is float)
                    {
                        cmd.Parameters.Add("param" + (i + 1), DbType.Single);
                        cmd.Parameters["param" + (i + 1)].Value = bindParameter[i];
                    }
                    // バイナリ
                    else if (bindParameter[i] is byte[])
                    {
                        cmd.Parameters.Add("param" + (i + 1), DbType.Byte);
                        cmd.Parameters["param" + (i + 1)].Value = bindParameter[i];
                    }
                }
            }
        }
    }

    /// <summary>
    /// SQLタイプ
    /// </summary>
    public enum SqlType
    {
        SELECT,
        INSERT,
        UPDATE,
        DELETE
    }
}
