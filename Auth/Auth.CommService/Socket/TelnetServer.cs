using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using System.Xml;
using System.Globalization;

namespace Auth.CommService
{
    public class TelnetServer : AppServer<TelnetSession>
    {
        //Telnet命令格式：Heartbeat:DeviceID|OtherInfo
        public TelnetServer()
            // :base(new TerminatorReceiveFilterFactory("##", Encoding.Default, new BasicRequestInfoParser(":", "|")))
             :base(new TerminatorReceiveFilterFactory("</Command>", Encoding.Default, new BasicRequestInfoParser("$#", "~")))
            // base(new TerminatorReceiveFilterFactory("</Command>"))
        {

        }
        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            return base.Setup(rootConfig, config);
        }

        protected override void OnStarted()
        {
            base.OnStarted();
        }

        protected override void OnStopped()
        {
            base.OnStopped();
        }

    }
}
