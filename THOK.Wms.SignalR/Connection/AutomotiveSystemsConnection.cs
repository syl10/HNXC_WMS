using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SignalR;

namespace THOK.Wms.SignalR.Connection
{
    public class AutomotiveSystemsConnection : PersistentConnection
    {
    }
    public class AutomotiveSystemsNotify
    {
        public static void Notify()
        {
            (new Notifier<AutomotiveSystemsConnection>()).Notify("TaskStart");
        }
    }
}
