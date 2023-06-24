using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SubelaServer
{
    public class User
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Network));
        public int Index;
        public string IP;
        public TcpClient Socket;
        public NetworkStream myStream;
        private byte[] readBuff;

        public void Start()
        {
            Socket.SendBufferSize = 4096;
            Socket.ReceiveBufferSize = 4096;

            myStream = Socket.GetStream();
            Array.Resize(ref readBuff, 4096);
            myStream.BeginRead(readBuff, 0, Socket.ReceiveBufferSize, onReceiveData, null);
        }

        void onReceiveData(IAsyncResult result)
        {
            try
            {
                int readBytes = myStream.EndRead(result);
                if (Socket == null) return;
                if(readBytes <= 0)
                {
                    closeConnection();
                    return;
                }

                byte[] newBytes = null;
                Array.Resize(ref newBytes, readBytes);

                Buffer.BlockCopy(readBuff, 0, newBytes, 0, readBytes);

                if(Socket== null) return;
                myStream.BeginRead(readBuff, 0, Socket.ReceiveBufferSize, onReceiveData, null);
            } catch (Exception e)
            {
                closeConnection();
                return;
            }
        }

        void closeConnection()
        {
            Socket.Close();
            Socket = null;
            Log.Error("User disconnect: " + IP);
        } 
    }
}
