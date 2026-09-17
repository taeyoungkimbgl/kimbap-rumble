using System;
using System.Runtime.Serialization;

namespace seiko.framework.bases.exception
{
    /// <summary>
    /// システムエラーのクラス
    /// </summary>
    [Serializable()] //クラスがシリアル化可能であることを示す属性
    class SystemErrorException : Exception
    {
        /// <summary>
        /// 例外コンストラクタ
        /// </summary>
        public SystemErrorException()
        : base()
        {
        }

        /// <summary>
        /// 例外コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        public SystemErrorException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 例外コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="innerException">発生済みの例外オブジェクト</param>
        public SystemErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        //逆シリアル化コンストラクタ。このクラスの逆シリアル化のために必須。
        //アクセス修飾子をpublicにしないこと！（詳細は後述）
        protected SystemErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}