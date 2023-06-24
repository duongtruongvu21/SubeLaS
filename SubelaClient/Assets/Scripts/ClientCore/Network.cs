using System;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class Network : MonoBehaviour
{
    public static Network Instance;

    [Header("Network Config")]
    public string serverIP = "127.0.0.1";
    public int serverPort = 13721;
    bool _isConnectted;
    public TcpClient socket;
    public NetworkStream myStream;
    public StreamReader myReader;
    public StreamWriter myWriter;

    public byte[] asyncBuff;
    public bool shouldHandleData;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        connectGameServer();
    }

    void connectGameServer()
    {
        if (socket != null)
        {
            if (socket.Connected || _isConnectted) return;
            socket.Close();
            socket = null;
        }

        socket = new TcpClient();
        socket.ReceiveBufferSize = 4096;
        socket.SendBufferSize = 4096;
        socket.NoDelay = false;
        Array.Resize(ref asyncBuff, 8192);
        socket.BeginConnect(serverIP, serverPort, new AsyncCallback(connectCallback), socket);
        _isConnectted = true;
    }

    void connectCallback(IAsyncResult result)
    {
        if (socket != null)
        {
            socket.EndConnect(result);
            if (!socket.Connected)
            {
                _isConnectted = false;
                return;
            }
        }
        else
        {
            socket.NoDelay = false;
            myStream = socket.GetStream();
            myStream.BeginRead(asyncBuff, 0, 8192, onReceive, null);
        }
    }

    void onReceive(IAsyncResult result)
    {
        if (socket != null)
        {
            if (socket == null) return;
            int byteArray = myStream.EndRead(result);
            byte[] myBytes = null;
            Array.Resize(ref myBytes, byteArray);

            Buffer.BlockCopy(asyncBuff, 0, myBytes, 0, byteArray);

            if (byteArray == 0)
            {
                Debug.Log("Disconnect from Server!!");
                socket.Close();
                return;
            }

            if (socket == null) return;

            myStream.BeginRead(asyncBuff, 0, 8192, onReceive, null);
        }
    }
}