using Microsoft.VisualStudio.TestTools.UnitTesting;

using SmsGatewayClient.Common;

namespace SmsGatewayClient.SMGP.Messages
{
    internal class SmgpLoginRespMessage : SmgpMessage
    {
        public SmgpLoginRespMessage()
        {
        }

        public SmgpLoginRespMessage(byte[] buffer)
            : base(buffer)
        {
            Assert.AreEqual(SmgpRequestId.Login_Resp, RequestId);
            Status = BitHelper.SubUInt32(buffer, HeaderSize);
            AuthenticatorServer = StringHelper.Hex(BitHelper.SubArray(buffer, HeaderSize + 4, 16));
            Version = buffer[HeaderSize + 20];
        }

        [ProtocolDesc(Name = "Status", Description = "Login请求返回结果", Size = 4)]
        public uint Status { get; set; }

        [ProtocolDesc(Name = "AuthenticatorServer", Description = "MD5（Status+AuthenticatorClient +shared secret）", Size = 16)]
        public string AuthenticatorServer { get; set; }

        [ProtocolDesc(Name = "Version", Description = "服务器支持的最高版本号", Size = 1)]
        public uint Version { get; set; }

        public override string ToString()
        {
            return string.Format("SMGP_Login_Resp:[SequenceID={0},Status={1},AuthenticatorServer={2},Version={3}]",
                                       SequenceId,
                                       Status,
                                       AuthenticatorServer,
                                       Version);
        }
    }
}
