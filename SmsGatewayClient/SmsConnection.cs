using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SmsGatewayClient.Common;
using SmsGatewayClient.Net;

namespace SmsGatewayClient
{
    /// <summary>
    /// 发送短信报文
    /// </summary>
    public abstract class SmsConnection : IDisposable
    {
        private const int Size = 100; // TODO config

        private static readonly byte[] sidLocker = new byte[0];
        private static long sequenceId;

        protected static readonly Random Random = new Random();

        private static readonly byte[] enterLocker = new byte[0];
        private static readonly Queue<SocketAsyncEventArgs> sendPool = new Queue<SocketAsyncEventArgs>();
        private static readonly Queue<SocketAsyncEventArgs> receivePool = new Queue<SocketAsyncEventArgs>();

        private static readonly byte[] msgLocker = new byte[0];
        private static readonly Dictionary<uint, WaitingDataToken> messageBuffer = new Dictionary<uint, WaitingDataToken>();

        internal static int trySending;
        internal static int totalSend;
        internal static int tryReceiving;
        internal static int totalReceived;

        protected SmsSocket socket;
        private bool disposed;

        /// <summary>
        /// 当前运行状态
        /// </summary>
        public string Status
        {
            get
            {
                return string.Format("Traffic:\t{0}\nTotal Send:\t{1} - {2}\nTotal Receive:\t{3} - {4}", socket.Traffic, trySending, totalSend, tryReceiving, totalReceived);
            }
        }

        /// <summary>
        /// 连接 socket
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        protected SmsConnection(string host, int port)
        {
            socket = SocketManager.Get(host, port);
            // 同一 IP 永远只属于一家运营商，则同一 Socket 上的心跳包格式一样；
            if (socket.KeepAlive == null)
            {
                socket.KeepAlive = new Thread(() =>
                    {
                        while (socket.Connected)
                        {
                            Heartbeat(socket);
                        }
                        socket.IsLogin = false;
                        Console.WriteLine("连接已断开。"); // TODO: 重连一定次数后 raise exception
                    });
            }
            new Thread(() =>
            {
                while (!disposed) { Receive(socket); Thread.Sleep(10); }
            }).Start();
        }

        /// <summary>
        /// 流水号
        /// </summary>
        /// <returns></returns>
        public static uint NextSequenceId()
        {
            lock (sidLocker)
            {
                sequenceId++;
                if (sequenceId > UInt32.MaxValue)
                {
                    sequenceId = 1;
                }
                return (uint)sequenceId;
            }
        }

        /// <summary>
        /// 向运营商服务器验证CP用户名密码
        /// </summary>
        /// <returns></returns>
        public int Login()
        {
            if (socket.IsLogin)
            {
                return 0;
            }

            lock (socket.Locker)
            {
                if (socket.IsLogin)
                {
                    return 0;
                }

                if (!socket.Connected)
                {
                    socket = SocketManager.Reconnect(socket);
                }

                int status = (int)LoginTemplate();

                socket.IsLogin = status == 0;

                if (socket.IsLogin)
                {
                    if (!socket.KeepAlive.IsAlive)
                    {
                        socket.KeepAlive = new Thread(() =>
                        {
                            while (socket.Connected)
                            {
                                Heartbeat(socket);
                            }
                            socket.IsLogin = false;
                            Console.WriteLine("连接已断开。"); // TODO: 重连一定次数后 raise exception
                        });
                        socket.KeepAlive.Start();
                    }
                }

                return status;
            }
        }

        /// <summary>
        /// 向运营商服务器验证CP用户名密码模板方法
        /// </summary>
        /// <returns></returns>
        public abstract uint LoginTemplate();

        /// <summary>
        /// 向运营商服务器发送短信
        /// </summary>
        /// <param name="phones">手机号列表</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public int Submit(string[] phones, string content)
        {
            if (!socket.Connected)
            {
                socket = SocketManager.Reconnect(socket);
            }

            if (!socket.IsLogin)
            {
                int status = Login();
                if (status != 0)
                {
                    return status;
                }
            }

            var messages = PackageMessages(phones, content);

            if (messages.Length == 1)
            {
                return (int)SubmitTemplate(messages[0]);
            }

            foreach (var message in messages)
            {
                SubmitTemplate(message);
            }

            return 0;
        }

        /// <summary>
        /// 生成发送报文
        /// </summary>
        /// <param name="phones">手机号</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        protected abstract SmsMessage[] PackageMessages(string[] phones, string content);

        /// <summary>
        /// 向运营商服务器发送短信模板方法
        /// </summary>
        /// <returns></returns>
        protected abstract uint SubmitTemplate(SmsMessage message);

        /// <summary>
        /// 向运营商发送链路检测包保持连接
        /// </summary>
        /// <param name="smsSocket"></param>
        protected abstract void Heartbeat(SmsSocket smsSocket);

        /// <summary>
        /// 运营商对同一连接上滑动时间窗（并发请求数量）有限制
        /// </summary>
        protected virtual int TrafficControl
        {
            get { return 16; }
        }

        /// <summary>
        /// 获取 SmsMessage 响应的 SequenceId
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected abstract uint ReadSequenceId(byte[] buffer);

        /// <summary>
        /// 解析请求报文
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected virtual SmsMessage Handle(byte[] buffer)
        {
            return null;
        }

        /// <summary>
        /// 异步发送不等待完成
        /// </summary>
        /// <param name="ack"></param>
        private void Send(SmsMessage ack)
        {
            SocketAsyncEventArgs sendArgs;
            try
            {
                SpinWait.SpinUntil(() => sendPool.Count < Size);

                lock (((ICollection)sendPool).SyncRoot)
                {
                    sendArgs = sendPool.Dequeue();
                }
            }
            catch (InvalidOperationException) // 队列为空
            {
                sendArgs = new SocketAsyncEventArgs();
                sendArgs.Completed += AfterSend;
            }

            var buffer = ack.ToBytes();
            sendArgs.SetBuffer(buffer, 0, buffer.Length); // 填充要发送的数据

            Interlocked.Increment(ref trySending);

            if (!socket.SendAsync(sendArgs)) // 未异步发送，应当在同步上下文中处理
            {
                AfterSend(null, sendArgs);
            }
        }

        /// <summary>
        /// 异步发送并且等待接收数据完成
        /// </summary>
        /// <param name="smsSocket"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected byte[] SendAndWait(SmsSocket smsSocket, SmsMessage message)
        {
            SocketAsyncEventArgs sendArgs;
            try
            {
                SpinWait.SpinUntil(() => sendPool.Count < Size);

                lock (enterLocker)
                {
                    SpinWait.SpinUntil(() => smsSocket.Traffic < TrafficControl); // 同一 Socket 上的流量控制
                    smsSocket.Traffic++; // 增加正在请求数量
                }

                lock (((ICollection)sendPool).SyncRoot)
                {
                    sendArgs = sendPool.Dequeue();
                }
            }
            catch (InvalidOperationException) // 队列为空
            {
                sendArgs = new SocketAsyncEventArgs();
                sendArgs.Completed += AfterSend;
            }

            // 发送完加入等待回复的队列
            var token = new WaitingDataToken { SequenceId = message.GetSequenceId() };
            lock (msgLocker)
            {
                messageBuffer.Add(token.SequenceId, token);
            }

            sendArgs.UserToken = token;
            var buffer = message.ToBytes();
            sendArgs.SetBuffer(buffer, 0, buffer.Length); // 填充要发送的数据

            Interlocked.Increment(ref trySending);

            if (!smsSocket.SendAsync(sendArgs)) // 未异步发送，应当在同步上下文中处理
            {
                AfterSend(null, sendArgs);
            }

            WaitHandle.WaitAll(new WaitHandle[] { token.WaitHandle }, 60 * 1000); // 等待异步请求结束
            Assert.IsTrue(socket.Traffic <= TrafficControl);

            Interlocked.Decrement(ref smsSocket.Traffic); // 减少正在请求数量
            lock (msgLocker)
            {
                messageBuffer.Remove(token.SequenceId);
            }

            if (token.Bytes == null)
            {
                token.SocketError = SocketError.TimedOut;
            }

            smsSocket.IsLogin = smsSocket.Connected;

            if (token.SocketError != SocketError.Success)
            {
                throw new SocketException((int)token.SocketError);
            }

            return token.Bytes;
        }

        /// <summary>
        /// 发送完 Message 后释放资源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void AfterSend(object sender, SocketAsyncEventArgs e)
        {
            Interlocked.Increment(ref totalSend);

            var token = (WaitingDataToken)e.UserToken;

            if (token != null)
            {
                if (e.SocketError != SocketError.Success)
                {
                    token.SocketError = e.SocketError;
                    token.WaitHandle.Set();
                }
            }

            SocketManager.Clear(e);
            lock (((ICollection)sendPool).SyncRoot)
            {
                sendPool.Enqueue(e);
            }
        }

        /// <summary>
        /// 开始接收数据
        /// </summary>
        /// <param name="smsSocket"></param>
        private void Receive(Socket smsSocket)
        {
            SocketAsyncEventArgs receiveArgs;
            try
            {
                SpinWait.SpinUntil(() => sendPool.Count < Size);

                lock (((ICollection)receivePool).SyncRoot)
                {
                    receiveArgs = receivePool.Dequeue(); // 从连接池中取出 receiveArgs
                }
            }
            catch (InvalidOperationException)
            {
                receiveArgs = new SocketAsyncEventArgs();
                receiveArgs.SetBuffer(new byte[64], 0, 64); // TODO: check size
                receiveArgs.Completed += AfterReceive;
            }

            Interlocked.Increment(ref tryReceiving);

            if (!smsSocket.ReceiveAsync(receiveArgs))
            {
                AfterReceive(null, receiveArgs);
            }
        }

        /// <summary>
        /// 接收 Message 的二进制数组
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AfterReceive(object sender, SocketAsyncEventArgs e)
        {
            uint seqId = ReadSequenceId(e.Buffer);

            if (seqId == 0)
            {
                return;
            }

            var bytes = new byte[e.BytesTransferred];
            Array.Copy(e.Buffer, e.Offset, bytes, 0, bytes.Length);

            if (!messageBuffer.ContainsKey(seqId))
            {
                var ack = Handle(bytes);
                if (ack != null)
                {
                    Send(ack);
                }
                return;
            }

            Interlocked.Increment(ref totalReceived);

            var token = messageBuffer[seqId];
            token.SocketError = e.SocketError;
            token.Bytes = bytes;
            token.WaitHandle.Set();

            SocketManager.Clear(e);
            lock (((ICollection)receivePool).SyncRoot)
            {
                receivePool.Enqueue(e);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            socket = null;
            disposed = true;
        }

        /// <summary>
        /// UDHI 长短信拆分
        /// </summary>
        /// <param name="message">短信</param>
        /// <param name="content">内容</param>
        /// <param name="no">第N条</param>
        /// <param name="count">共M条</param>
        /// <param name="udhiId">本条消息唯一标识</param>
        /// <param name="limit">字数限制（默认140）</param>
        /// <param name="headLength">udhi头长度（默认为6）</param>
        protected virtual void Udhi(ISubmitMessage message, byte[] content, int no, int count, byte udhiId, int limit = 140, int headLength = 6)
        {
            if (count == 1)
            {
                message.MsgContent = content;
            }
            else // 长短信
            {
                var index = no*limit - headLength;

                var length = Math.Min(limit - headLength, content.Length - index);

                message.TpUdhi = 1;
                message.MsgContent = new byte[length + headLength];
                message.MsgContent[0] = 0x05;
                message.MsgContent[2] = 0x03;
                message.MsgContent[3] = udhiId;
                message.MsgContent[4] = (byte)count;
                message.MsgContent[5] = (byte)(no + 1);

                if (no == 0)
                {
                    Array.Copy(content, 0, message.MsgContent, headLength, length);
                }
                else
                {
                    Array.Copy(content, index, message.MsgContent, headLength, length);
                }

            }

            message.MsgLength = (uint)message.MsgContent.Length;
        }
    }
}