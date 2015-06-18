using Microsoft.VisualStudio.TestTools.UnitTesting;

using SmsGatewayClient.Common;

namespace SmsGatewayClient.CMPP.Messages
{
    internal class CmppConnectRespMessage : CmppMessage
    {
        public CmppConnectRespMessage()
        {
        }

        public CmppConnectRespMessage(byte[] buffer)
            : base(buffer)
        {
            Assert.AreEqual(CmppCommandId.CMPP_CONNECT_RESP, CommandId);
            Status = buffer[HeaderSize];
            AuthenticatorISMG = StringHelper.Hex(BitHelper.SubArray(buffer, HeaderSize + 1, 16));
            Version = buffer[HeaderSize + 17];
        }

        [ProtocolDesc(Name = "Status", Description = "状态：0-正确，1-消息结构错，2-非法源地址，3-认证错，4-版本太高，5~-其他错误", Size = 1)]
        public uint Status { get; set; }

        [ProtocolDesc(Name = "AuthenticatorISMG", Description = "ISMG认证码，MD5(Status+AuthenticatorSource+shared secret)", Size = 16)]
        public string AuthenticatorISMG { get; set; }

        [ProtocolDesc(Name = "Version", Description = "服务器支持的最高版本号", Size = 1)]
        public uint Version { get; set; }

        public override string ToString()
        {
            return string.Format("CMPP_CONNECT_RESP:[Sequence_Id={0},Status={1},AuthenticatorISMG={2},Version={3}]",
                                       SequenceId,
                                       Status,
                                       AuthenticatorISMG,
                                       Version);
        }
    }
}