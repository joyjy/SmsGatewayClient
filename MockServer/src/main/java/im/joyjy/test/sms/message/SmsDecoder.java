package im.joyjy.test.sms.message;

import java.util.List;

import io.netty.buffer.ByteBuf;
import io.netty.channel.ChannelHandlerContext;
import io.netty.handler.codec.ByteToMessageDecoder;

public class SmsDecoder extends ByteToMessageDecoder {

	@Override
	protected void decode(ChannelHandlerContext ctx, ByteBuf in,
			List<Object> out) throws Exception {

		if (in.readableBytes() < 12) {
			return;
		}

		in.markReaderIndex();

		SmgpSmsMessage message = new SmgpSmsMessage();

		message.setPacketLength(in.readInt());

		if (in.readableBytes() < message.getPacketLength() - 4) {
			in.resetReaderIndex();
			return;
		}

		message.setRequestId(in.readInt());
		message.setSequenceId(in.readInt());
		out.add(message);

		byte[] body = new byte[message.getPacketLength() - 12];
		in.readBytes(body);
	}

}
