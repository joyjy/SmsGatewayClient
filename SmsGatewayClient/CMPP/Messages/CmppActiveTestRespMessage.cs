using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmsGatewayClient.Common;

namespace SmsGatewayClient.CMPP.Messages
{
    internal class CmppActiveTestRespMessage : CmppMessage
    {
        public CmppActiveTestRespMessage()
        {
            CommandId = CmppCommandId.CMPP_ACTIVE_TEST_RESP;
        }

        public CmppActiveTestRespMessage(byte[] buffer)
            : base(buffer)
        {
            Assert.AreEqual(CmppCommandId.CMPP_ACTIVE_TEST_RESP, CommandId);
            Reserved = buffer[HeaderSize];
        }

        [ProtocolDesc(Name = "Reserved", Size = 1)]
        public uint Reserved { get; set; }

        public override string ToString()
        {
            return string.Format("CMPP_ACTIVE_TEST_RESP:[Sequence_Id={0},Reserved={1}]", SequenceId, Reserved);
        }
    }
}