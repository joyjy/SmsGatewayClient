using Microsoft.VisualStudio.TestTools.UnitTesting;

using SmsGatewayClient.Common;

namespace SmsGatewayClient.SMGP.Messages
{
    class SmgpDeliverMessage : SmgpMessage
    {
        private string msgIdBCD;

        public SmgpDeliverMessage(byte[] buffer)
        {
            Assert.AreEqual(SmgpRequestId.Deliver, RequestId);

            MsgId = BitHelper.SubArray(buffer, HeaderSize, 10);

            msgIdBCD = "";
            msgIdBCD += BitHelper.BCD(BitHelper.SubArray(buffer, HeaderSize, 3));
            msgIdBCD += "-" + BitHelper.BCD(BitHelper.SubArray(buffer, HeaderSize + 3, 4));
            msgIdBCD += "-" + BitHelper.BCD(BitHelper.SubArray(buffer, HeaderSize + 7, 3));

            IsReport = BitHelper.SubUInt32(buffer, HeaderSize + 10, 1);
            MsgFormat = BitHelper.SubUInt32(buffer, HeaderSize + 11, 1);
            RecvTime = System.Text.Encoding.Default.GetString(buffer, HeaderSize + 12, 14);
            SrcTermId = System.Text.Encoding.Default.GetString(buffer, HeaderSize + 26, 21);
            DestTermId = System.Text.Encoding.Default.GetString(buffer, HeaderSize + 47, 21);
            MsgLength = BitHelper.SubUInt32(buffer, HeaderSize + 68, 1);
            MsgContent = System.Text.Encoding.Default.GetString(buffer, HeaderSize + 69, (int) MsgLength);
        }

        [ProtocolDesc(Name = "MsgID", Size = 10, Description = "短消息流水号")]
        public byte[] MsgId { get; set; }

        [ProtocolDesc(Name = "IsReport", Size = 1, Description = "是否为状态报告")]
        public uint IsReport { get; set; }

        [ProtocolDesc(Name = "MsgFormat", Size = 1, Description = "短消息格式")]
        public uint MsgFormat { get; set; }

        [ProtocolDesc(Name = "RecvTime", Size = 14, Description = "短消息接收时间")]
        public string RecvTime { get; set; }

        [ProtocolDesc(Name = "SrcTermID", Size = 21, Description = "短消息发送号码")]
        public string SrcTermId { get; set; }

        [ProtocolDesc(Name = "DestTermID", Size = 21, Description = "短消息接收号码")]
        public string DestTermId { get; set; }

        [ProtocolDesc(Name = "MsgLength", Size = 1, Description = "短消息长度")]
        public uint MsgLength { get; set; }

        [ProtocolDesc(Name = "MsgContent", Size = 1, MultiBy = "MsgLength", Description = "短消息内容")]
        public string MsgContent { get; set; }

        [ProtocolDesc(Name = "Reserve ", Size = 8, Description = "保留")]
        public string Reserve { get; set; }

        public override string ToString()
        {
            return string.Format(@"SMGP_Deliver:[SequenceID={0},MsgId={1},IsReport={2},MsgFormat={3},RecvTime={4},
SrcTermID={5},DestTermID={6},MsgLength={7},MsgContent={8},Reserve={9}]",
                                    SequenceId, MsgId, IsReport, msgIdBCD, RecvTime, SrcTermId, DestTermId, MsgLength, MsgContent, Reserve);
        
        }
    }
}
