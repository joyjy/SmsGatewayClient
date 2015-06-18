namespace SmsGatewayClient.SMGP.Messages
{
    internal struct SmgpRequestId
    {
        public const uint Login = 0x00000001; //客户端登录
        public const uint Login_Resp = 0x80000001; //客户端登录应答
        public const uint Submit = 0x00000002; //提交短消息
        public const uint Submit_Resp = 0x80000002; //提交短消息应答
        public const uint Deliver = 0x00000003; //下发短消息
        public const uint Deliver_Resp = 0x80000003; //下发短消息应答
        public const uint Active_Test = 0x00000004; //链路检测
        public const uint Active_Test_Resp = 0x80000004; //链路检测应答
        public const uint Forward = 0x00000005; //短消息前转
        public const uint Forward_Resp = 0x80000005; //短消息前转应答
        public const uint Exit = 0x00000006; //退出请求
        public const uint Exit_Resp = 0x80000006; //退出应答
        public const uint Query = 0x00000007; //SP 统计查询
        public const uint Query_Resp = 0x80000007; //SP 统计查询应答
        public const uint Query_TE_Route = 0x00000008; //查询 TE 路由
        public const uint Query_TE_Route_Resp = 0x80000008; //查询 TE 路由应答
        public const uint Query_SP_Route = 0x00000009; //查询 SP 路由
        public const uint Query_SP_Route_Resp = 0x80000009; //查询 SP 路由应答
        public const uint Payment_Request = 0x0000000A; //扣款请求(用于预付费系统，参见增值业务计费方案)
        public const uint Payment_Request_Resp = 0x8000000A; //扣款请求响应(用于预付费系统，参见增值业务计费方案，下同)
        public const uint Payment_Affirm = 0x0000000B; //扣款确认(用于预付费系统，参见增值业务计费方案)
        public const uint Payment_Affirm_Resp = 0x8000000B; //扣款确认响应(用于预付费系统，参见增值业务计费方案)
        public const uint Query_UserState = 0x0000000C; //查询用户状态(用于预付费系统，参见增值业务计费方案)
        public const uint Query_UserState_Resp = 0x8000000C; //查询用户状态响应(用于预付费系统，参见增值业务计费方案) 
        public const uint Get_All_TE_Route = 0x0000000D; //获取所有终端路由
        public const uint Get_All_TE_Route_Resp = 0x8000000D; //获取所有终端路由应答
        public const uint Get_All_SP_Route = 0x0000000E; //获取所有 SP 路由
        public const uint Get_All_SP_Route_Resp = 0x8000000E; //获取所有 SP 路由应答
        public const uint Update_TE_Route = 0x0000000F; //SMGW 向 GNS 更新终端路由
        public const uint Update_TE_Route_Resp = 0x8000000F; //SMGW 向 GNS 更新终端路由应答
        public const uint Update_SP_Route = 0x00000010; //SMGW 向 GNS 更新 SP 路由
        public const uint Update_SP_Route_Resp = 0x80000010; //SMGW 向 GNS 更新 SP 路由应答
        public const uint Push_Update_TE_Route = 0x00000011; //GNS 向 SMGW 更新终端路由
        public const uint Push_Update_TE_Route_Resp = 0x80000011; //GNS 向 SMGW 更新终端路由应答
        public const uint Push_Update_SP_Route = 0x00000012; //GNS 向 SMGW 更新 SP 路由
        public const uint Push_Update_SP_Route_Resp = 0x80000012; //GNS 向 SMGW 更新 SP 路由应答
    }
}
