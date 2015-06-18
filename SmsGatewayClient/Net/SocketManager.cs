using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SmsGatewayClient.Net
{
    /// <summary>
    /// Socket 管理
    /// </summary>
    public class SocketManager
    {
        private static readonly byte[] locker = new byte[0];

        private static readonly Dictionary<string, SmsSocket> sockets = new Dictionary<string, SmsSocket>();

        /// <summary>
        /// 获取指定主机的 Socket 连接
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static SmsSocket Get(string host, int port)
        {
            var key = host + port;
            if (sockets.ContainsKey(key))
            {
                return sockets[key];
            }

            lock (locker)
            {
                if (sockets.ContainsKey(key))
                {
                    return sockets[key];
                }

                var socket = new SmsSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(host, port);
                sockets.Add(key, socket);
                return socket;
            }
        }

        /// <summary>
        /// 连接断开后只能重连
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public static SmsSocket Reconnect(SmsSocket socket)
        {
            var newSocket = new SmsSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    KeepAlive = socket.KeepAlive
                };
            newSocket.Connect(socket.RemoteEndPoint);
            sockets[socket.RemoteEndPoint.ToString()] = newSocket;
            return newSocket;
        }

        /// <summary>
        /// 清除上次请求的数据
        /// </summary>
        /// <param name="args"></param>
        public static void Clear(SocketAsyncEventArgs args)
        {
            Array.Clear(args.Buffer, 0, args.Buffer.Length); // 清空之前的数据
            args.UserToken = null;
            args.AcceptSocket = null;
        }
    }
}