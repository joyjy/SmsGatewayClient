using SmsGatewayClient.Common;

namespace SmsGatewayClient.SMGP.Messages
{
    internal abstract class SmgpMessage : SmsMessage
    {
        protected SmgpMessage()
        {
        }

        protected SmgpMessage(byte[] buffer)
        {
            if (buffer.Length == 0)
            {
                return;
            }
            PacketLength = BitHelper.SubUInt32(buffer, 0);
            RequestId = BitHelper.SubUInt32(buffer, 4);
            SequenceId = BitHelper.SubUInt32(buffer, SequenceIdIndex);
        }

        public const int HeaderSize = 12;

        public const int SequenceIdIndex = 8;

        public const int RequestIdIndex = 4;

        [ProtocolDesc(Name = "PacketLength", Size = 4, Description = "数据包长度")]
        public uint PacketLength { get; set; }

        [ProtocolDesc(Name = "RequestID", Size = 4, Description = "请求标识")]
        public uint RequestId { get; set; }

        [ProtocolDesc(Name = "SequenceID", Size = 4, Description = "序列号")]
        public uint SequenceId { get; set; }

        public override uint GetSequenceId()
        {
            return SequenceId;
        }

        public override byte[] ToBytes()
        {
            var body = GetBody();
            var message = new byte[body.Length + HeaderSize];
            PacketLength = (uint)message.Length;

            var index = BitHelper.Padding(PacketLength, message, 4);
            index = BitHelper.Padding(RequestId, message, index, 4);
            index = BitHelper.Padding(SequenceId, message, index, 4);
            BitHelper.Padding(body, message, index, body.Length);

            return message;
        }

        public abstract override string ToString();
    }
}