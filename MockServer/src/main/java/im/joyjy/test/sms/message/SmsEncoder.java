package im.joyjy.test.sms.message;

import io.netty.buffer.ByteBuf;
import io.netty.channel.ChannelHandlerContext;
import io.netty.handler.codec.MessageToByteEncoder;


public class SmsEncoder extends MessageToByteEncoder<SmgpSmsMessage> {

	@Override
	protected void encode(ChannelHandlerContext ctx, SmgpSmsMessage msg,
			ByteBuf out) throws Exception {
		
		out.writeInt(12);
		out.writeInt(msg.getRequestId() | 0x80000000);
		out.writeInt(msg.getSequenceId());
	}

}
