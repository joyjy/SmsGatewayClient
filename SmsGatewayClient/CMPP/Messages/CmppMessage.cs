using SmsGatewayClient.Common;

namespace SmsGatewayClient.CMPP.Messages
{
    internal abstract class CmppMessage : SmsMessage
    {
        protected CmppMessage()
        {
        }

        protected CmppMessage(byte[] buffer)
        {
            if (buffer.Length == 0)
            {
                return;
            }
            TotalLength = BitHelper.SubUInt32(buffer, 0);
            CommandId = BitHelper.SubUInt32(buffer, 4);
            SequenceId = BitHelper.SubUInt32(buffer, SequenceIdIndex);
        }

        public const int HeaderSize = 12;
        public const int CommandIdIndex = 4;
        public const int SequenceIdIndex = 8;

        [ProtocolDesc(Name = "Total_Length", Description = "总长度", Size = 4)]
        public uint TotalLength { get; set; }

        [ProtocolDesc(Name = "Command_Id", Description = "命令或响应类型", Size = 4)]
        public uint CommandId { get; set; }

        [ProtocolDesc(Name = "Sequence_Id", Description = "消息流水号", Size = 4)]
        public uint SequenceId { get; set; }

        public override uint GetSequenceId()
        {
            return SequenceId;
        }

        public override byte[] ToBytes()
        {
            var body = GetBody();
            var message = new byte[body.Length + HeaderSize];
            TotalLength = (uint)message.Length;

            var index = BitHelper.Padding(TotalLength, message, 4);
            index = BitHelper.Padding(CommandId, message, index, 4);
            index = BitHelper.Padding(SequenceId, message, index, 4);
            BitHelper.Padding(body, message, index, body.Length);

            return message;
        }
    }
}