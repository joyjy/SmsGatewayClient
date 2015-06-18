package im.joyjy.test.sms;

import im.joyjy.test.sms.message.SmsDecoder;
import im.joyjy.test.sms.message.SmsEncoder;
import io.netty.bootstrap.ServerBootstrap;
import io.netty.channel.ChannelFuture;
import io.netty.channel.ChannelInitializer;
import io.netty.channel.ChannelOption;
import io.netty.channel.ChannelPipeline;
import io.netty.channel.EventLoopGroup;
import io.netty.channel.nio.NioEventLoopGroup;
import io.netty.channel.socket.SocketChannel;
import io.netty.channel.socket.nio.NioServerSocketChannel;

public class NettyServerBootstrap {
	
    public static void main(String []args) throws Exception{

        ServerBootstrap bootstrap=new ServerBootstrap();
        
    	EventLoopGroup boss=new NioEventLoopGroup();
        EventLoopGroup worker=new NioEventLoopGroup();
        bootstrap.group(boss,worker);
        
        bootstrap.channel(NioServerSocketChannel.class);
        bootstrap.option(ChannelOption.SO_BACKLOG, 128);
        bootstrap.option(ChannelOption.TCP_NODELAY, true);
        bootstrap.childOption(ChannelOption.SO_KEEPALIVE, true);
        bootstrap.childHandler(new ChannelInitializer<SocketChannel>() {
            @Override
            protected void initChannel(SocketChannel socketChannel) throws Exception {
                ChannelPipeline p = socketChannel.pipeline();
                p.addLast("encoder", new SmsEncoder());
                p.addLast("decoder", new SmsDecoder());
                p.addLast("nettyServerHandler", new NettyServerHandler());
            }
        });
        
        ChannelFuture f= bootstrap.bind(9890).sync();
        
        if(f.isSuccess()){
            System.out.println("Server start at 9890.");
        }
        
        while (true){
        	Thread.sleep(10);
        }
    }
}
