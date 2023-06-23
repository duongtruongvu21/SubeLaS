using log4net.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SubelaServer
{
    class Network
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Network));

        public TcpListener ServerSocket;
        public static Network Instance = new Network();
        public static User[] User = new User[100];

        public void ServerStart()
        {
            for(int i = 0; i < User.Length; i++)
            {
                User[i] = new User();
            }
            ServerSocket = new TcpListener(IPAddress.Any, 5500);
            ServerSocket.Start();
            ServerSocket.BeginAcceptTcpClient(OnClientConnect, null);

            log.Debug(456);
            log.Debug(456/(456 + 456 - 456 / 0.5 ));
        }

        void OnClientConnect(IAsyncResult result)
        {
            TcpClient client = ServerSocket.EndAcceptTcpClient(result);
            client.NoDelay = false;

            ServerSocket.BeginAcceptTcpClient(OnClientConnect, null);
        }
    }
}
