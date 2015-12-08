using System.Net.Sockets;
using System.Threading;

namespace SmsGatewayClient.Net
{
    /// <summary>
    /// 用于发短信的 Socket
    /// </summary>
    public class SmsSocket : Socket
    {
        /// <summary>
        /// 
        /// </summary>
        public volatile byte[] Locker = new byte[0];

        /// <summary>
        /// 当前 Socket 上尚未回复的短信数量
        /// </summary>
        private readonly Semaphore traffic;

        /// <summary>
        /// 短信流量限制
        /// </summary>
        private readonly int trafficControl;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addressFamily"></param>
        /// <param name="socketType"></param>
        /// <param name="protocolType"></param>
        /// <param name="trafficControl"></param>
        public SmsSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType, int trafficControl) 
            : base(addressFamily, socketType, protocolType)
        {
            this.trafficControl = trafficControl;
            traffic = new Semaphore(trafficControl, trafficControl);
        }

        /// <summary>
        /// 是否已经登录
        /// </summary>
        public bool IsLogin { get; set; }

        /// <summary>
        /// 用于发送心跳包的线程
        /// </summary>
        public Thread KeepAlive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TrafficControl
        {
            get { return trafficControl; }
        }

        /// <summary>
        /// 
        /// </summary>
        public void WaitTraffic()
        {
            traffic.WaitOne();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReleaseTraffic()
        {
            traffic.Release();
        }
    }
}