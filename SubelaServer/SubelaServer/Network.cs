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
        public static List<Client> Clients;

        public void ServerStart()
        {
            Clients = new List<Client>();
            ServerSocket = new TcpListener(IPAddress.Any, 13721);
            ServerSocket.Start();
            ServerSocket.BeginAcceptTcpClient(OnClientConnect, null);
        }

        void OnClientConnect(IAsyncResult result)
        {
            TcpClient client = ServerSocket.EndAcceptTcpClient(result);
            client.NoDelay = false;

            ServerSocket.BeginAcceptTcpClient(OnClientConnect, null);

            int clientCount = Clients.Count;
            for (int j = 0; j < clientCount; j++)
            {
                Clients[j].Socket = client;
                Clients[j].Index = j;
                Clients[j].IP = client.Client.RemoteEndPoint.ToString();
                Clients[j].Start();
                Log.Info($"Client connect from {Clients[j].IP}, index = {Clients[j].Index}");
                return;
            }

            Client newClient = new Client();
            newClient.Socket = client;
            newClient.Index = Clients.Count;
            newClient.IP = client.Client.RemoteEndPoint.ToString();
            newClient.Start();
            Log.Info($"Client connect from {newClient.IP}, index = {newClient.Index}");
        }
    }
}