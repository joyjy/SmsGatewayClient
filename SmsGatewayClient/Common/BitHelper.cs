using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SmsGatewayClient.Common
{
    /// <summary>
    /// 字节数组处理辅助方法
    /// </summary>
    public static class BitHelper
    {
        /// <summary>
        /// 将源数据填充到目标字节数组的指定块
        /// </summary>
        /// <param name="source">源数据</param>
        /// <param name="dest">目标字节数组</param>
        /// <param name="size">块大小</param>
        /// <returns>下一处起始索引</returns>
        public static int Padding(uint source, byte[] dest, int size)
        {
            return Padding(source, dest, 0, size);
        }

        /// <summary>
        /// 将源数据填充到目标字节数组的指定块
        /// </summary>
        /// <param name="source">源数据</param>
        /// <param name="dest">目标字节数组</param>
        /// <param name="fromIndex">起始索引</param>
        /// <param name="size">块大小</param>
        /// <returns>下一处起始索引</returns>
        public static int Padding(uint source, byte[] dest, int fromIndex, int size)
        {
            byte[] sourceArray = BitConverter.GetBytes(source);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(sourceArray);
            }
            Array.Copy(sourceArray, sourceArray.Length - size, dest, fromIndex, size);
            return fromIndex + size;
        }

        /// <summary>
        /// 将源数据填充到目标字节数组的指定块
        /// </summary>
        /// <param name="source">源数据</param>
        /// <param name="dest">目标字节数组</param>
        /// <param name="size">块大小</param>
        /// <returns>下一处起始索引</returns>
        public static int Padding(string source, byte[] dest, int size)
        {
            return Padding(source, dest, 0, size);
        }

        /// <summary>
        /// 将源数据填充到目标字节数组的指定块
        /// </summary>
        /// <param name="source">源数据</param>
        /// <param name="dest">目标字节数组</param>
        /// <param name="fromIndex">起始索引</param>
        /// <param name="size">块大小</param>
        /// <returns>下一处起始索引</returns>
        public static int Padding(string source, byte[] dest, int fromIndex, int size)
        {
            byte[] sourceArray = Encoding.UTF8.GetBytes(source);
            Array.Copy(sourceArray, 0, dest, fromIndex, size);
            return fromIndex + size;
        }

        /// <summary>
        /// 将源数据填充到目标字节数组的指定块
        /// </summary>
        /// <param name="source">源数据</param>
        /// <param name="dest">目标字节数组</param>
        /// <param name="fromIndex">起始索引</param>
        /// <returns>下一处起始索引</returns>
        public static int Padding(byte[] source, byte[] dest, int fromIndex)
        {
            Array.Copy(source, 0, dest, fromIndex, source.Length);
            return fromIndex + source.Length;
        }

        /// <summary>
        /// 将源数据填充到目标字节数组的指定块
        /// </summary>
        /// <param name="source">源数据</param>
        /// <param name="dest">目标字节数组</param>
        /// <param name="fromIndex">起始索引</param>
        /// <param name="size">块大小</param>
        /// <returns>下一处起始索引</returns>
        public static int Padding(byte[] source, byte[] dest, int fromIndex, int size)
        {
            if (source == null)
            {
                source = new byte[0];
            }

            var copySize = Math.Min(source.Length, size);
            Array.Copy(source, 0, dest, fromIndex, copySize);

            return fromIndex + size;
        }

        /// <summary>
        /// 从二进制数组中截取数组
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="fromIndex"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static byte[] SubArray(byte[] buffer, int fromIndex, int size)
        {
            var sub = new byte[size];
            Array.Copy(buffer, fromIndex, sub, 0, size);
            return sub;
        }

        /// <summary>
        /// 从二进制数组中截取 uint32 值
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static uint SubUInt32(byte[] buffer, int index)
        {
            return SubUInt32(buffer, index, 4);
        }

        /// <summary>
        /// 从二进制数组中截取 uint32 值
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static uint SubUInt32(byte[] buffer, int index, int length)
        {
            var num = SubArray(buffer, index, length);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(num);
            }
            return BitConverter.ToUInt32(num, 0);
        }

        /// <summary>
        /// 从二进制数组中截取 uint64 值
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static ulong SubUInt64(byte[] buffer, int index)
        {
            var num = SubArray(buffer, index, 8);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(num);
            }
            return BitConverter.ToUInt64(num, 0);
        }

        /// <summary>
        /// 转换为协议二进制
        /// </summary>
        /// <param name="message">对象</param>
        /// <returns></returns>
        public static byte[] ToProtocolBinaryArray(SmsMessage message)
        {
            var type = message.GetType();
            var props = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);

            int length = 0;
            var paramList = new List<ProtocolParam>();
            foreach (var property in props)
            {
                var protocols = property.GetCustomAttributes(typeof(ProtocolDescAttribute), false);
                if (protocols.Length == 0)
                {
                    continue;
                }

                var desc = (ProtocolDescAttribute)protocols[0];
                var param = new ProtocolParam
                    {
                        Name = desc.Name,
                        Size = desc.MultiBy == null
                                ? desc.Size
                                : desc.Size * Value(paramList, desc) // 会根据其他值的变化而变化
                    };

                if (desc.Tag != 0) // TLV Format
                {
                    var tlv = GetTlvValue(message, property, desc.Tag, desc.Size);
                    if (tlv == null)
                    {
                        continue;
                    }
                    param.Value = tlv;
                    param.Size += 4;
                }
                else
                {
                    param.Value = GetValue(message, property, desc.Size, param.Size);
                }
                paramList.Add(param);
                length += param.Size;
            }

            var result = new byte[length];
            int index = 0;
            foreach (var param in paramList)
            {
                index = Padding(param.Value, result, index, param.Size);
            }
            return result;
        }

        /// <summary>
        /// 从 List 里查找 ProtocolDesc MultiBy 的参数对象
        /// </summary>
        /// <param name="paramList"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        private static int Value(List<ProtocolParam> paramList, ProtocolDescAttribute desc)
        {
            var num = paramList.Find(p => p.Name == desc.MultiBy).Value;
            return num[0];
        }

        /// <summary>
        /// 转换对象的属性值为二进制
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="property">指定属性值</param>
        /// <param name="singleSize">属性结果数组的指定长度</param>
        /// <param name="totalSize">多个参数的属性结果数组的最终长度</param>
        /// <returns></returns>
        private static byte[] GetValue(object o, PropertyInfo property, int singleSize, int totalSize)
        {
            var value = property.GetValue(o, null);
            switch (property.PropertyType.Name)
            {
                case "UInt32":
                    byte[] array = BitConverter.GetBytes((uint)value);
                    Array.Resize(ref array, singleSize);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(array);
                    }
                    return array;
                case "String":
                    if (value == null)
                    {
                        return new byte[0];
                    }
                    return Encoding.Default.GetBytes((string)value);
                case "Byte[]":
                    return (byte[])value;
                case "String[]":
                    var stringArray = value as String[];
                    if (stringArray == null || stringArray.Length == 0)
                    {
                        return new byte[0];
                    }
                    array = new byte[totalSize];
                    int i = 0;
                    for (int index = 0; index < totalSize; index += singleSize)
                    {
                        var singleBytes = Encoding.Default.GetBytes(stringArray[i++]);
                        Array.Copy(singleBytes, 0, array, index, singleBytes.Length);
                    }
                    return array;

            }
            return null;
        }

        /// <summary>
        /// 获取 TLV 对象值
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="property">属性</param>
        /// <param name="tag">TAG值</param>
        /// <param name="size">VALUE指定长度</param>
        /// <returns></returns>
        private static byte[] GetTlvValue(object o, PropertyInfo property, int tag, int size)
        {
            var value = property.GetValue(o, null);
            switch (property.PropertyType.Name)
            {
                case "UInt32":
                    if ((uint)value == 0)
                    {
                        return null;
                    }
                    byte[] array = BitConverter.GetBytes((uint)value);
                    Array.Resize(ref array, size);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(array);
                    }
                    var result = new byte[size + 4];
                    Padding((uint)tag, result, 2);
                    Padding((uint)size, result, 2, 2);
                    Padding(array, result, 4, size);
                    return result;
            }

            return null;
        }

        /// <summary>
        /// 协议参数
        /// </summary>
        private class ProtocolParam
        {
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 值
            /// </summary>
            public byte[] Value { get; set; }

            /// <summary>
            /// 长度
            /// </summary>
            public int Size { get; set; }
        }

        /// <summary>
        /// BCD 码
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string BCD(byte[] num)
        {
            var result = "";
            foreach (var b in num)
            {
                int valueH = (b & 0xF0) >> 4;
                int valueL = b & 0x0F;
                result += valueH;
                result += valueL;
            }
            return result;
        }
    }
}
