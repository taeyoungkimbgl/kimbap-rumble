using System;
using System.Runtime.Serialization;

namespace seiko.framework.bases.exception
{
    /// <summary>
    /// 業務エラーのクラス
    /// </summary>
    [Serializable()] //クラスがシリアル化可能であることを示す属性
    public class GyomuException : Exception
    {
        /// <summary>
        /// 例外コンストラクタ
        /// </summary>
        public GyomuException()
        : base()
        {
        }

        /// <summary>
        /// 例外コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        public GyomuException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 例外コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="innerException">発生済みの例外オブジェクト</param>
        public GyomuException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        //逆シリアル化コンストラクタ。このクラスの逆シリアル化のために必須。
        //アクセス修飾子をpublicにしないこと！（詳細は後述）
        protected GyomuException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}