using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabaAOSServer
{
    class SocketServer
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Work();
        }
    }
}
