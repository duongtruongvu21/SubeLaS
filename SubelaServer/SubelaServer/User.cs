using DataCore.Helpers;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

                byte[] receivedBytes = new byte[readBytes];
                Buffer.BlockCopy(readBuff, 0, receivedBytes, 0, readBytes);

                Log.Info("test");
                Dictionary<byte, object> receivedData = DeserializeData(receivedBytes);
                Log.Info(receivedData[1]);

                if (Socket== null) return;
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

        public void SendData(Dictionary<byte, object> data)
        {
            if (Socket == null || myStream == null)
            {
                Log.Error("Socket or network stream is null");
                return;
            }

            try
            {
                byte[] bytes = NetworkHelper.SerializeData(data);
                myStream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception e)
            {
                Log.Error("Error sending data: " + e.Message);
            }
        }

        Dictionary<byte, object> DeserializeData(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<Dictionary<byte, object>>(json);
        }
    }
}