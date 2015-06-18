# SmsGatewayClient
C# CMPP2.0，SMGP3.0 客户端

## 说明

1. SmsGatewayClient 为 .Net 4.0 Framework 类库，使用方法：
	```
	using(var conn = new CmppConnection(ip, port, cp, pwd, serviceId, appPhoneNo)){
		conn.Submit(new[] { "13810000000" }, "测试")
	}
	
	using(var conn = new SmgpConnection(ip, port, cp, pwd, serviceId, appPhoneNo)){
		conn.Submit(new[] { "18910000000" }, "测试")
	}
	
	```
2. MockServer 为基于 Netty 的模拟服务器，目前可返回 SMGP 头。
