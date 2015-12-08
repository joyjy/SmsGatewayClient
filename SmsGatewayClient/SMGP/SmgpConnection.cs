using System;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SmsGatewayClient.Common;
using SmsGatewayClient.Net;
using SmsGatewayClient.SMGP.Messages;

namespace SmsGatewayClient.SMGP
{
    /// <summary>
    /// 中国电信短信（Short Message Peer to Peer）SMGP协议实现
    /// </summary>
    public class SmgpConnection : SmsConnection
    {
        private readonly string cpId;
        private readonly string password;
        private readonly string appCode;
        private readonly string appPhone;

        public SmgpConnection(string host, int port, string cpId, string password, string appCode, string appPhone)
            : base(host, port)
        {
            this.cpId = cpId;
            this.password = password;
            this.appCode = appCode;
            this.appPhone = appPhone;
        }

        /// <summary>
        /// 获取 SmsMessage 响应的 SequenceId
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected override uint ReadSequenceId(byte[] buffer)
        {
            return BitHelper.SubUInt32(buffer, SmgpMessage.SequenceIdIndex);
        }

        /// <summary>
        /// 向运营商服务器验证CP用户名密码模板方法
        /// </summary>
        /// <returns></returns>
        public override uint LoginTemplate()
        {
            var timestamp = DateTime.Now.ToString("MMddHHmmss");
            var message = new SmgpLoginMessage
            {
                SequenceId = NextSequenceId(),
                ClientId = cpId,
                AuthenticatorClient = SmgpLoginMessage.Sign(cpId, password, timestamp),
                TimeStamp = (uint)Convert.ToInt32(timestamp),
                Version = 30,
            };

            var resp = new SmgpLoginRespMessage(SendAndWait(socket, message));
            Assert.AreEqual(message.SequenceId, resp.SequenceId);

            return resp.Status;
        }

        /// <summary>
        /// 生成发送报文
        /// </summary>
        /// <param name="phones">手机号</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        protected override SmsMessage[] PackageMessages(string[] phones, string content)
        {
            var targetCount = (phones.Length - 1) / 100 + 1; // 群发短信最多支持100条

            var contentBytes = SmsMessage.Ucs2Encoding.GetBytes(content);
            var contentCount = (contentBytes.Length - 1) / 140 + 1; // 短信内容最多支持140字节

            var result = new SmgpSubmitMessage[targetCount * contentCount];

            for (int i = 0; i < targetCount; i++)
            {
                var udhiId = (byte)Random.Next(byte.MaxValue);
                for (int j = 0; j < contentCount; j++)
                {
                    var message = new SmgpSubmitMessage
                    {
                        SequenceId = NextSequenceId(),
                        MsgType = 6,
                        ServiceId = appCode,
                        FeeType = "00",
                        FeeCode = "0",
                        FixedFee = "0",
                        MsgFormat = 8,
                        SrcTermId = appPhone,
                        DestTermIdCount = (uint)Math.Min(100, phones.Length - i * 100),
                        PkTotal = (uint)contentCount,
                        PkNumber = (uint)(j + 1)
                    };
                    message.DestTermId = new string[message.DestTermIdCount];
                    Array.Copy(phones, i * 100, message.DestTermId, 0, (int)message.DestTermIdCount);
                    Udhi(message, contentBytes, j, contentCount, udhiId);

                    result[i * j + j] = message;
                }
            }

            return result;
        }

        /// <summary>
        /// 向运营商服务器发送短信模板方法
        /// </summary>
        /// <returns></returns>
        protected override uint SubmitTemplate(SmsMessage message)
        {
            var resp = new SmgpSubmitRespMessage(SendAndWait(socket, message));
            Assert.AreEqual(((SmgpSubmitMessage) message).SequenceId, resp.SequenceId);

            return resp.Status;
        }

        /// <summary>
        /// 向运营商发送链路检测包保持连接
        /// </summary>
        /// <param name="smsSocket"></param>
        protected override void Heartbeat(SmsSocket smsSocket)
        {
            var message = new SmgpActiveTestMessage
            {
                SequenceId = NextSequenceId()
            };

            var resp = new SmgpActiveTestRespMessage(SendAndWait(smsSocket, message));
            Assert.AreEqual(message.SequenceId, resp.SequenceId);

            Thread.Sleep(3 * 60 * 1000); // TODO: 配置
        }

        /// <summary>
        /// 解析请求报文
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected override SmsMessage Handle(byte[] buffer)
        {
            uint requestId = BitHelper.SubUInt32(buffer, SmgpMessage.RequestIdIndex);
            switch (requestId)
            {
                case SmgpRequestId.Active_Test:
                    {
                        var message = new SmgpActiveTestMessage(buffer);
                        var ack = new SmgpActiveTestRespMessage
                            {
                                SequenceId = message.SequenceId
                            };
                        return ack;
                    }
                case SmgpRequestId.Deliver:
                    {
                        var message = new SmgpDeliverMessage(buffer);
                        var ack = new SmgpDeliverRespMessage
                        {
                            SequenceId = message.SequenceId,
                            MsgId = message.MsgId,
                            Status = 0
                        };
                        return ack;
                    }
                default:
                    return null;// throw new NotImplementedException("UnHandleRequest: " + requestId);
            }
        }
    }
}