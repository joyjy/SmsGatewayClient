using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmsGatewayClient.SMGP.Messages
{
    internal class SmgpActiveTestRespMessage : SmgpMessage
    {
        public SmgpActiveTestRespMessage()
        {
            RequestId = SmgpRequestId.Active_Test_Resp;
        }

        public SmgpActiveTestRespMessage(byte[] buffer)
            : base(buffer)
        {
            Assert.AreEqual(SmgpRequestId.Active_Test_Resp, RequestId);
        }

        public override string ToString()
        {
            return string.Format("SMGP_Active_Test_Resp:[SequenceID={0}]", SequenceId);
        }
    }
}