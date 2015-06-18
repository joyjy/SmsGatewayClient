using Microsoft.VisualStudio.TestTools.UnitTesting;

using SmsGatewayClient.Common;

namespace SmsGatewayClient.CMPP.Messages
{
    internal class CmppSubmitRespMessage : CmppMessage
    {
        public CmppSubmitRespMessage(byte[] buffer) : base(buffer)
        {
            Assert.AreEqual(CmppCommandId.CMPP_SUBMIT_RESP, CommandId);
            MsgId = BitHelper.SubUInt64(buffer, HeaderSize);
            Result = buffer[HeaderSize + 8];
        }

        [ProtocolDesc(Name = "Msg_Id", Description = "信息标识，由时间、短信网关代码和序列号组成", Size = 8)]
        public ulong MsgId { get; set; }

        [ProtocolDesc(Name = "Result", Description = "结果：0-正确，1-消息结构错，2-命令字错，3-消息序号重复，4-消息长度错，5-资费代码错，6-超过最大信息长，7-业务代码错，8-流量控制错，9~：其他错误", Size = 1)]
        public uint Result { get; set; }

        public override string ToString()
        {
            return string.Format("CMPP_CONNECT_RESP:[Sequence_Id={0},Msg_Id={1},Result={2}]",
                                       SequenceId,
                                       MsgId,
                                       Result);
        }
    }
}