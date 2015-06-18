using System.Net.Sockets;
using System.Threading;

namespace SmsGatewayClient.Common
{
    /// <summary>
    /// 存储异步请求返回值的对象
    /// </summary>
    internal class WaitingDataToken
    {
        public WaitingDataToken()
        {
            WaitHandle = new ManualResetEvent(false);
        }

        /// <summary>
        /// 是否已完成
        /// </summary>
        public ManualResetEvent WaitHandle { get; set; }

        /// <summary>
        /// 请求唯一标识
        /// </summary>
        public uint SequenceId { get; set; }

        /// <summary>
        /// 请求中的 SocketError
        /// </summary>
        public SocketError SocketError { get; set; }

        /// <summary>
        /// 返回结果
        /// </summary>
        public byte[] Bytes { get; set; }
    }
}