using Microsoft.VisualStudio.TestTools.UnitTesting;

using SmsGatewayClient.Common;

namespace SmsGatewayClient.SMGP.Messages
{
    internal class SmgpSubmitRespMessage : SmgpMessage
    {
        public SmgpSubmitRespMessage(byte[] buffer)
            : base(buffer)
        {
            Assert.AreEqual(SmgpRequestId.Submit_Resp, RequestId);
            var msgId = "";
            msgId += BitHelper.BCD(BitHelper.SubArray(buffer, HeaderSize, 3));
            msgId += "-" + BitHelper.BCD(BitHelper.SubArray(buffer, HeaderSize + 3, 4));
            msgId += "-" + BitHelper.BCD(BitHelper.SubArray(buffer, HeaderSize + 7, 3));
            MsgId = msgId;
            Status = BitHelper.SubUInt32(buffer, HeaderSize + 10);
        }

        [ProtocolDesc(Name = "MsgID", Size = 10, Description = "信息标识，由短信网关代码、时间和序列号组成")]
        public string MsgId { get; set; }

        [ProtocolDesc(Name = "Status", Size = 4, Description = "Submit请求返回结果")]
        public uint Status { get; set; }

        public override string ToString()
        {
            return string.Format("SMGP_Submit_Resp:[SequenceID={0},MsgID={1},Status={2}]",
                                       SequenceId,
                                       MsgId,
                                       Status);
        }
    }
}
