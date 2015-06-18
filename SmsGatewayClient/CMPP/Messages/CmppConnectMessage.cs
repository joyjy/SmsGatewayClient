using System.Security.Cryptography;
using System.Text;

using SmsGatewayClient.Common;

namespace SmsGatewayClient.CMPP.Messages
{
    internal class CmppConnectMessage : CmppMessage
    {
        public CmppConnectMessage()
        {
            CommandId = CmppCommandId.CMPP_CONNECT;
        }

        [ProtocolDesc(Name = "Source_Addr", Description = "源地址即SP_Id", Size = 6)]
        public string SourceAddr { get; set; }

        [ProtocolDesc(Name = "AuthenticatorSource", Description = "MD5(Source_Addr+9字节的0 +shared secret+timestamp)", Size = 16)]
        public byte[] AuthenticatorSource { get; set; }

        [ProtocolDesc(Name = "Version", Description = "版本号", Size = 1)]
        public uint Version { get; set; }

        [ProtocolDesc(Name = "Timestamp", Description = "时间戳，MMddHHmmss", Size = 4)]
        public uint Timestamp { get; set; }

        protected override byte[] GetBody()
        {
            return BitHelper.ToProtocolBinaryArray(this);
        }

        public static byte[] Sign(string cpId, string password, string timestamp)
        {
            var secret = Encoding.UTF8.GetBytes(password);
            var timestampBytes = Encoding.UTF8.GetBytes(timestamp);

            var origin = new byte[6 + 9 + secret.Length + timestampBytes.Length];

            var index = BitHelper.Padding(cpId, origin, 6) + 9;
            index = BitHelper.Padding(secret, origin, index);
            index = BitHelper.Padding(timestampBytes, origin, index);

            return MD5.Create().ComputeHash(origin);
        }

        public override string ToString()
        {
            return string.Format("CMPP_CONNECT:[Sequence_Id={0},Source_Addr={1},AuthenticatorSource={2},Version={3},Timestamp={4}]",
                                    SequenceId,
                                    SourceAddr,
                                    StringHelper.Hex(AuthenticatorSource),
                                    Version,
                                    Timestamp);
        }
    }
}