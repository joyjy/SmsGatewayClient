using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmsGatewayClient.SMGP.Messages
{
    internal class SmgpActiveTestMessage : SmgpMessage
    {
        public SmgpActiveTestMessage()
        {
            RequestId = SmgpRequestId.Active_Test;
        }

        public SmgpActiveTestMessage(byte[] buffer):base(buffer)
        {
            Assert.AreEqual(SmgpRequestId.Active_Test, RequestId);
        }

        public override string ToString()
        {
            return string.Format("SMGP_Active_Test:[SequenceID={0}]", SequenceId);
        }
    }
}
