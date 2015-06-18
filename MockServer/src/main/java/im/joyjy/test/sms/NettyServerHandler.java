package im.joyjy.test.sms;

import im.joyjy.test.sms.message.SmgpSmsMessage;
import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.SimpleChannelInboundHandler;
import io.netty.util.ReferenceCountUtil;

import java.util.Random;

public class NettyServerHandler extends SimpleChannelInboundHandler<SmgpSmsMessage> {
	
	private static Random random = new Random();

	@Override
	protected void channelRead0(ChannelHandlerContext ctx, SmgpSmsMessage msg)
			throws Exception {

		System.out.println("receive: " + msg);
		Thread.sleep(random.nextInt(50));

		SmgpSmsMessage ack = new SmgpSmsMessage();
		ack.setPacketLength(msg.getPacketLength());
		ack.setRequestId(msg.getRequestId());
		ack.setSequenceId(msg.getSequenceId());

		ctx.channel().writeAndFlush(ack);

		ReferenceCountUtil.release(msg);
	}
}
