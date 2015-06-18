using System;

namespace SmsGatewayClient.Common
{
    /// <summary>
    /// ProtocolDesc 属性
    /// </summary>
    internal class ProtocolDescAttribute : Attribute
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 指定大小
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 长度为 Size 倍数的对象名称
        /// </summary>
        public string MultiBy { get; set; }

        /// <summary>
        /// TLV 专用
        /// </summary>
        public int Tag { get; set; }
    }
}