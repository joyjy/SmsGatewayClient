using System.Text;

namespace SmsGatewayClient
{
    public abstract class SmsMessage
    {
        /// <summary>
        /// UCS2编码
        /// </summary>
        public static readonly Encoding Ucs2Encoding = Encoding.BigEndianUnicode;

        /// <summary>
        /// 获取 GetSequenceId
        /// </summary>
        /// <returns></returns>
        public abstract uint GetSequenceId();

        /// <summary>
        /// 转换为二进制
        /// </summary>
        /// <returns></returns>
        public abstract byte[] ToBytes();

        /// <summary>
        /// 报文内容的二进制
        /// </summary>
        /// <returns></returns>
        protected virtual byte[] GetBody() { return new byte[0]; }

        /// <summary>
        /// 中文短信使用的编码 UCS2
        /// </summary>
        protected internal virtual Encoding Encoding { get { return Ucs2Encoding; } }
    }
}