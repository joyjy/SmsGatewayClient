using System.Net.Sockets;
using System.Threading;

namespace SmsGatewayClient.Net
{
    /// <summary>
    /// 用于发短信的 Socket
    /// </summary>
    public class SmsSocket : Socket
    {
        public volatile byte[] Locker = new byte[0];

        /// <summary>
        /// 当前 Socket 上尚未回复的短信数量
        /// </summary>
        public int Traffic;

        public SmsSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType) 
            : base(addressFamily, socketType, protocolType)
        {
        }

        /// <summary>
        /// 是否已经登录
        /// </summary>
        public bool IsLogin { get; set; }

        /// <summary>
        /// 用于发送心跳包的线程
        /// </summary>
        public Thread KeepAlive { get; set; }
    }
}