using SmsGatewayClient.Common;

namespace SmsGatewayClient.SMGP.Messages
{
    class SmgpDeliverRespMessage : SmgpMessage
    {
        public SmgpDeliverRespMessage()
        {
            RequestId = SmgpRequestId.Deliver_Resp;
        }

        [ProtocolDesc(Name = "MsgID", Size = 10, Description = "短消息流水号")]
        public byte[] MsgId { get; set; }

        [ProtocolDesc(Name = "Status", Size = 4, Description = "请求返回结果")]
        public uint Status { get; set; }

        protected override byte[] GetBody()
        {
            return BitHelper.ToProtocolBinaryArray(this);
        }

        public override string ToString()
        {
            return string.Format("SMGP_Deliver_Resp:[SequenceID={0},MsgID={1},Status={2}]",
                                       SequenceId,
                                       MsgId,
                                       Status);
        }
    }
}
