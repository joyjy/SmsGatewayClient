using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmsGatewayClient.CMPP.Messages
{
    internal class CmppActiveTestMessage : CmppMessage
    {
        public CmppActiveTestMessage()
        {
            CommandId = CmppCommandId.CMPP_ACTIVE_TEST;
        }

        public CmppActiveTestMessage(byte[] buffer)
            : base(buffer)
        {
            Assert.AreEqual(CmppCommandId.CMPP_ACTIVE_TEST, CommandId);
        }

        public override string ToString()
        {
            return string.Format("CMPP_ACTIVE_TEST:[Sequence_Id={0}]", SequenceId);
        }
    }
}
