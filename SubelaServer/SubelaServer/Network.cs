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
        private static readonly ILog Log = LogManager.GetLogger(typeof(Network));

        public TcpListener ServerSocket;
        public static Network Instance = new Network();
        public static User[] User = new User[100];

        public void ServerStart()
        {
            for(int i = 0; i < User.Length; i++)
            {
                User[i] = new User();
            }
            ServerSocket = new TcpListener(IPAddress.Any, 13721);
            ServerSocket.Start();
            ServerSocket.BeginAcceptTcpClient(OnClientConnect, null);

            Log.Debug(456);
        }

        void OnClientConnect(IAsyncResult result)
        {
            TcpClient client = ServerSocket.EndAcceptTcpClient(result);
            client.NoDelay = false;

            Log.Debug(999);
            ServerSocket.BeginAcceptTcpClient(OnClientConnect, null);

            for (int i = 0; i < User.Length; i++)
            {
                User[i].Socket = client;
                User[i].Index = i;
                User[i].IP = client.Client.RemoteEndPoint.ToString();
                User[i].Start();
                Log.Info($"Client connect from {User[i].IP}, index = {User[i].Index}");
                return;
            }
        }
    }
}
