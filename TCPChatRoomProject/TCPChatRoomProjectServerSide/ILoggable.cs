using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPChatRoomProjectServerSide
{
    public interface ILoggable
    {
        void AddToLog(string message);
        void AddToErrorLog(string message);
        string GetLog();

        
    }
}
