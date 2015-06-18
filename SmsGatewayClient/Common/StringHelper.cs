using System;
using System.Collections.Generic;
using System.Linq;

namespace SmsGatewayClient.Common
{
    public class StringHelper
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        /// <param name="destTerminalId"></param>
        /// <returns></returns>
        public static string Contact(IEnumerable<string> destTerminalId)
        {
            var result = destTerminalId.Aggregate("", (current, id) => current + ("," + id));
            return result.Remove(0, 1);
        }

        /// <summary>
        /// 十六进制字符串
        /// </summary>
        /// <param name="authenticatorSource"></param>
        /// <returns></returns>
        public static string Hex(byte[] authenticatorSource)
        {
            return BitConverter.ToString(authenticatorSource).Replace("-", "").ToLower();
        }
    }
}
