using System.Security.Cryptography;
using System.Text;

using SmsGatewayClient.Common;

namespace SmsGatewayClient.SMGP.Messages
{
    internal class SmgpLoginMessage : SmgpMessage
    {
        public SmgpLoginMessage()
        {
            RequestId = SmgpRequestId.Login;
        }

        [ProtocolDesc(Name = "ClientID", Size = 8, Description = "CP编号")]
        public string ClientId { get; set; }

        [ProtocolDesc(Name = "AuthenticatorClient", Size = 16, Description = "MD5（ClientID+7 字节的0 +shared secret+timestamp）")]
        public byte[] AuthenticatorClient { get; set; }

        [ProtocolDesc(Name = "LoginMode", Size = 1, Description = "登录类型（0=发送短消息, 1=接收短消息，2=收发短消息，其他保留）")]
        public uint LoginMode { get; set; }

        [ProtocolDesc(Name = "TimeStamp", Size = 4, Description = "时间戳的明文,由客户端产生,格式为MMDDHHMMSS，即月日时分秒，10位数字的整型,右对齐")]
        public uint TimeStamp { get; set; }

        [ProtocolDesc(Name = "Version", Size = 1, Description = "客户端支持的版本号(高位4bit表示主版本号,低位4bit表示次版本号)")]
        public uint Version { get; set; }

        protected override byte[] GetBody()
        {
            return BitHelper.ToProtocolBinaryArray(this);
        }

        internal static byte[] Sign(string cpId, string password, string timestamp)
        {
            var secret = Encoding.UTF8.GetBytes(password);
            var timestampBytes = Encoding.UTF8.GetBytes(timestamp);

            var origin = new byte[8 + 7 + secret.Length + timestampBytes.Length];

            var index = BitHelper.Padding(cpId, origin, 8) + 7;
            index = BitHelper.Padding(secret, origin, index);
            index = BitHelper.Padding(timestampBytes, origin, index);

            return MD5.Create().ComputeHash(origin);
        }

        public override string ToString()
        {
            return string.Format("SMGP_Login:[SequenceID={0},ClientID={1},AuthenticatorClient={2},LoginMode={3},Timestamp={4},Version={5}]",
                                    SequenceId,
                                    ClientId,
                                    StringHelper.Hex(AuthenticatorClient),
                                    LoginMode,
                                    TimeStamp,
                                    Version);
        }
    }
}
