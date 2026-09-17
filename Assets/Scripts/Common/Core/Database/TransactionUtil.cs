using Mono.Data.Sqlite;
// using seiko.framework.bases.config;
using seiko.framework.bases.exception;
using System;
using System.Threading;


namespace seiko.framework.bases.db
{
    /// <summary>
    /// DBトランザクションクラス
    /// </summary>
    public static class TransactionUtil
    {

        // トランザクションインスタンスを生成 
        private static SqliteTransaction Transaction;

        // 設定情報
        private static readonly DbConfig DbConfig;

        // セマフォのインスタンス生成
        private static SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        // セマフォ取得の待機時間
        private static readonly int TransactionWaitTime = DbConfig.TransactionWaitTime;

        /// <summary>
        /// トランザクション開始
        /// </summary>
        public static void StartTransaction()
        {
            try
            {
                // セマフォを要求　取得できなければ処理を止めてコンフィグで設定したミリ秒だけ待機
                // 取得できた場合はtrue、出来なかった場合はfalse
                bool SemaphoreFlag = Semaphore.Wait(TransactionWaitTime);

                // セマフォが取得できなかった場合
                if (!SemaphoreFlag)
                {
                    throw new GyomuException("排他制御エラー");
                }

                // トランザクション開始
                Transaction = DbConnectUtil.Connection.BeginTransaction();
            }
            catch (GyomuException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new SystemErrorException("トランザクション開始エラー", e);
            }
        }

        /// <summary>
        /// コミット
        /// </summary>
        public static void Commit()
        {
            Transaction.Commit();

            // セマフォを解放
            Semaphore.Release();
        }

        /// <summary>
        /// ロールバック
        /// </summary>
        public static void Rollback()
        {
            Transaction.Rollback();

            // セマフォを解放
            Semaphore.Release();
        }

        /// <summary>
        /// トランザクション状態チェック
        /// </summary>
        public static bool isTransctionOpen()
        {
            if (Transaction.Connection != null && Transaction.Connection.State.ToString() == "Open")
            {
                return true;
            }
            return false;
        }
    }
}