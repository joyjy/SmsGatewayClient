namespace SmsGatewayClient
{
    /// <summary>
    /// 发送短信
    /// </summary>
    public interface ISubmitMessage
    {
        /// <summary>
        /// 短信内容
        /// </summary>
        byte[] MsgContent { get; set; }

        /// <summary>
        /// TP_udhi
        /// </summary>
        uint TpUdhi { get; set; }

        /// <summary>
        /// 短信长度
        /// </summary>
        uint MsgLength { get; set; }

        /// <summary>
        /// 长短信序号
        /// </summary>
        uint PkNumber { get; set; }

        /// <summary>
        /// 长短信条数
        /// </summary>
        uint PkTotal { get; set; }
    }
}