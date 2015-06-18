using SmsGatewayClient.Common;

namespace SmsGatewayClient.CMPP.Messages
{
    internal class CmppSubmitMessage : CmppMessage, ISubmitMessage
    {
        public CmppSubmitMessage()
        {
            CommandId = CmppCommandId.CMPP_SUBMIT;
        }

        [ProtocolDesc(Name = "Msg_Id", Description = "信息标识", Size = 8)]
        public ulong MsgId { get; set; }

        [ProtocolDesc(Name = "Pk_total", Description = "相同 Msg_Id 的消息总条数", Size = 1)]
        public uint PkTotal { get; set; }

        [ProtocolDesc(Name = "Pk_number", Description = "相同 Msg_Id 的消息序号", Size = 1)]
        public uint PkNumber { get; set; }

        [ProtocolDesc(Name = "Registered_Delivery", Description = "是否要求返回状态确认报告：0-不需要，1-需要，2-产生SMC话单", Size = 1)]
        public uint RegisteredDelivery { get; set; }

        [ProtocolDesc(Name = "Msg_level", Description = "信息级别", Size = 1)]
        public uint MsgLevel { get; set; }

        [ProtocolDesc(Name = "Service_Id", Description = "业务类型", Size = 10)]
        public string ServiceId { get; set; }

        [ProtocolDesc(Name = "Fee_UserType", Description = "计费用户类型字段：0-对目的终端MSISDN计费，1-对源终端MSISDN计费，2-对SP计费，3-表示本字段无效，对谁计费参见Fee_terminal_Id字段", Size = 1)]
        public uint FeeUserType { get; set; }

        [ProtocolDesc(Name = "Fee_terminal_Id", Description = "被计费用户的号码", Size = 21)]
        public uint FeeTerminalId { get; set; }

        [ProtocolDesc(Name = "TP_pId", Description = "GSM协议类型03.40.9.2.3.9", Size = 1)]
        public uint TpPid { get; set; }

        [ProtocolDesc(Name = "TP_udhi", Description = "GSM协议类型03.40.9.2.3.23", Size = 1)]
        public uint TpUdhi { get; set; }

        [ProtocolDesc(Name = "Msg_Fmt", Description = "信息格式：0-ASCII串，3-短信写卡操作，4-二进制信息，8-UCS2编码，15-含GB汉字", Size = 1)]
        public uint MsgFmt { get; set; }

        [ProtocolDesc(Name = "Msg_src", Description = "信息内容来源(SP_Id)", Size = 6)]
        public string MsgSrc { get; set; }

        [ProtocolDesc(Name = "FeeType", Description = "资费类别：01-对“计费用户号码”免费，02-对“计费用户号码”按条计信息费，03-对“计费用户号码”按包月收取信息费，04-对“计费用户号码”的信息费封顶，05-对“计费用户号码”的收费是由SP实现", Size = 2)]
        public string FeeType { get; set; }

        [ProtocolDesc(Name = "FeeCode", Description = "资费代码（以分为单位）", Size = 6)]
        public string FeeCode { get; set; }

        [ProtocolDesc(Name = "ValId_Time", Description = "存活有效期，SMPP3.3", Size = 17)]
        public string VaildTime { get; set; }

        [ProtocolDesc(Name = "At_Time", Description = "定时发送时间，SMPP3.3", Size = 17)]
        public string AtTime { get; set; }

        [ProtocolDesc(Name = "Src_Id", Description = "源号码", Size = 21)]
        public string SrcId { get; set; }

        [ProtocolDesc(Name = "DestUsr_tl", Description = "接收信息的用户数量(小于100)", Size = 1)]
        public uint DestUserTl { get; set; }

        [ProtocolDesc(Name = "Dest_terminal_Id", Description = "接收短信的MSISDN号码", Size = 21, MultiBy = "DestUsr_tl")]
        public string[] DestTerminalId { get; set; }

        [ProtocolDesc(Name = "Msg_Length", Description = "信息长度(Msg_Fmt值为0时：<160个字节；其它<=140个字节)", Size = 1)]
        public uint MsgLength { get; set; }

        [ProtocolDesc(Name = "Msg_Content", Description = "信息内容", Size = 1, MultiBy = "Msg_Length")]
        public byte[] MsgContent { get; set; }

        [ProtocolDesc(Name = "Reserve", Description = "保留", Size = 8)]
        public string Reserve { get; set; }

        protected override byte[] GetBody()
        {
            return BitHelper.ToProtocolBinaryArray(this);
        }

        public override string ToString()
        {
            return string.Format(@"CMPP_SUBMIT:[Sequence_Id={0},Msg_Id={1},Pk_total={2},Pk_number={3},Registered_Delivery={4},Msg_level={5},
Service_Id={6},Fee_UserType={7},Fee_terminal_Id={8},TP_pId={9},TP_udhi={10},Msg_Fmt={11},Msg_src={12},FeeType={13},FeeCode={14},
ValId_Time={15},At_Time={16},Src_Id={17},DestUsr_tl={18},Dest_terminal_Id={19},Msg_Length={20},Msg_Content={21},Reserve={22}]",
                                    SequenceId, MsgId, PkTotal, PkNumber, RegisteredDelivery, MsgLevel, ServiceId, FeeUserType,
                                    FeeTerminalId, TpPid, TpUdhi, MsgFmt, MsgSrc, FeeType, FeeCode, VaildTime, AtTime, SrcId, 
                                    DestUserTl, StringHelper.Contact(DestTerminalId), MsgLength, Encoding.GetString(MsgContent), Reserve);
        }
    }
}