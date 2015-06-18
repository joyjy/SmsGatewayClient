using SmsGatewayClient.Common;

namespace SmsGatewayClient.SMGP.Messages
{
    internal class SmgpSubmitMessage : SmgpMessage, ISubmitMessage
    {
        public SmgpSubmitMessage()
        {
            RequestId = SmgpRequestId.Submit;
        }

        [ProtocolDesc(Name = "MsgType", Size = 1, Description = "短消息类型（1＝取消订阅，2＝订阅请求，3＝点播，4＝订阅，5＝交互式操作，6=查询，其他保留）")]
        public uint MsgType { get; set; }

        [ProtocolDesc(Name = "NeedReport", Size = 1, Description = "是否要求返回状态报告（0=不要求，1=要求）")]
        public uint NeedReport { get; set; }

        [ProtocolDesc(Name = "Priority", Size = 1, Description = "发送优先级（从0到9）")]
        public uint Priority { get; set; }

        [ProtocolDesc(Name = "ServiceID", Size = 10, Description = "业务类型")]
        public string ServiceId { get; set; }

        [ProtocolDesc(Name = "FeeType", Size = 2, Description = "收费类型（参见第7.2节收费类型代码表）")]
        public string FeeType { get; set; }

        [ProtocolDesc(Name = "FeeCode", Size = 6, Description = "资费代码（单位为分）")]
        public string FeeCode { get; set; }

        [ProtocolDesc(Name = "FixedFee", Size = 6, Description = "包月费/封顶费（单位为分）")]
        public string FixedFee { get; set; }

        [ProtocolDesc(Name = "MsgFormat", Size = 1, Description = "短消息格式（参见第7.1节短消息格式代码表）")]
        public uint MsgFormat { get; set; }

        [ProtocolDesc(Name = "ValidTime", Size = 17, Description = "有效时间，格式遵循SMPP3.3协议")]
        public string ValidTime { get; set; }

        [ProtocolDesc(Name = "AtTime", Size = 17, Description = "定时发送时间，格式遵循SMPP3.3协议")]
        public string AtTime { get; set; }

        [ProtocolDesc(Name = "SrcTermID", Size = 21, Description = "短信息发送方的电话号码（格式为“电话号码*子信箱号“），当短消息来自CP时，电话号码为118+CP ID或者发送方主叫号码。")]
        public string SrcTermId { get; set; }

        [ProtocolDesc(Name = "ChargeTermID", Size = 21, Description = "计费用户号码")]
        public string ChargeTermId { get; set; }

        [ProtocolDesc(Name = "DestTermIDCount", Size = 1, Description = "短消息接收号码总数（≤100）")]
        public uint DestTermIdCount { get; set; }

        [ProtocolDesc(Name = "DestTermID", Size = 21, MultiBy = "DestTermIDCount", Description = "短消息接收号码（连续存储DestTermIDCount个号码，每一个接收方号码的的最长长度为21")]
        public string[] DestTermId { get; set; }

        [ProtocolDesc(Name = "MsgLength", Size = 1, Description = "短消息长度")]
        public uint MsgLength { get; set; }

        [ProtocolDesc(Name = "MsgContent", Size = 1, MultiBy = "MsgLength", Description = "短消息内容")]
        public byte[] MsgContent { get; set; }

        [ProtocolDesc(Name = "Reserve", Size = 8, Description = "保留")]
        public string Reserve { get; set; }

        [ProtocolDesc(Name = "TP_udhi", Tag = 0x0002, Size = 1, Description = "GSM 协议类型 7.3.3")]
        public uint TpUdhi { get; set; }

        [ProtocolDesc(Name = "PkTotal", Tag = 0x0009, Size = 1, Description = "相同 MsgID 的消息总条数")]
        public uint PkTotal { get; set; }

        [ProtocolDesc(Name = "PkNumber", Tag = 0x000A, Size = 1, Description = "相同 MsgID 的消息序号")]
        public uint PkNumber { get; set; }

        protected override byte[] GetBody()
        {
            return BitHelper.ToProtocolBinaryArray(this);
        }

        public override string ToString()
        {
            return string.Format(@"SMGP_Submit:[SequenceID={0},MsgType={1},NeedReport={2},Priority={3},ServiceID={4},
FeeType={5},FeeCode={6},FixedFee={7},MsgFormat={8},ValidTime={9},AtTime={10},SrcTermID={11},ChargeTermID={12},
DestTermIDCount={13},DestTermID={14},MsgLength={15},MsgContent={16},Reserve={17},TP_udhi={18},PkTotal={19},PkNumber={20}]",
                                    SequenceId, MsgType, NeedReport, Priority, ServiceId, FeeType, FeeCode, FixedFee, MsgFormat,
                                    ValidTime, AtTime, SrcTermId, ChargeTermId, DestTermIdCount, StringHelper.Contact(DestTermId),
                                    MsgLength, Encoding.GetString(MsgContent), Reserve, TpUdhi, PkTotal, PkNumber);
        }
    }
}
