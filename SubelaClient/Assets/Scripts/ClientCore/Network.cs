using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class Network : MonoBehaviour
{
    public static Network Instance;

    [Header("Network Config")]
    public string serverIP = "127.0.0.1";
    public int serverPort = 13721;
    bool _isConnected;
    TcpClient socket;
    NetworkStream myStream;
    StreamReader myReader;
    StreamWriter myWriter;

    byte[] asyncBuff;
    bool shouldHandleData;
    bool isClosingConnection;

    private void Awake()
    {
        Instance = this;
    }

    private async void Start()
    {
        await ConnectToServerAsync();
    }

    async Task ConnectToServerAsync()
    {
        try
        {
            socket = new TcpClient();
            socket.ReceiveBufferSize = 4096;
            socket.SendBufferSize = 4096;
            socket.NoDelay = false;
            asyncBuff = new byte[8192];

            await socket.ConnectAsync(serverIP, serverPort);
            Debug.Log("Connected to the server!");

            myStream = socket.GetStream();
            if (socket.Connected)
            {
                myStream.BeginRead(asyncBuff, 0, 8192, OnReceive, null);
                _isConnected = true;
            }
        }
        catch (Exception e)
        {
            _isConnected = false;
            Debug.Log("Failed to connect to the server: " + e.Message);
        }
    }

    void OnReceive(IAsyncResult result)
    {
        if (socket == null || myStream == null) return;

        try
        {
            int byteArray = myStream.EndRead(result);
            if (byteArray == 0)
            {
                Debug.Log("Disconnected from the server!");
                CloseConnection();
                return;
            }

            byte[] myBytes = new byte[byteArray];
            Buffer.BlockCopy(asyncBuff, 0, myBytes, 0, byteArray);

            // Xử lý dữ liệu nhận được
            // ...

            myStream.BeginRead(asyncBuff, 0, 8192, OnReceive, null);
        }
        catch (Exception e)
        {
            Debug.Log("Error receiving data: " + e.Message);
            CloseConnection();
        }
    }

    private void Update()
    {
        if (_isConnected && socket != null)
        {
            if (!socket.Connected && !isClosingConnection)
            {
                // Server tắt
                Debug.Log("Server has been disconnected!");
                CloseConnection();
            }
        }
    }

    private void CloseConnection()
    {
        isClosingConnection = true;
        _isConnected = false;

        if (myStream != null)
        {
            myStream.Close();
            myStream = null;
        }

        if (myReader != null)
        {
            myReader.Close();
            myReader = null;
        }

        if (myWriter != null)
        {
            myWriter.Close();
            myWriter = null;
        }

        if (socket != null)
        {
            socket.Close();
            socket = null;
        }
    }

    public void SendData(Dictionary<byte, object> data, bool isTCP = true)
    {
        if (!_isConnected || socket == null || myStream == null)
        {
            Debug.Log("Not connected to the server!");
            return;
        }
        byte[] bytes = SerializeData(data);

        if (isTCP)
        {
            myStream.Write(bytes, 0, bytes.Length);
        }
        else
        {
            socket.Client.SendTo(bytes, socket.Client.RemoteEndPoint);
        }
    }

    public void SendTest(string input = "Vũ Yêu Thảo")
    {
        //for(int i = 0; i < 10; i++)
        //{
        //    var data = new Dictionary<byte, object>();
        //    data[1] = input + $" {i}!";

        //    SendData(data);
        //}

        StartCoroutine(send(input, 0));
    }

    IEnumerator send(string input, int i)
    {
        yield return new WaitForSeconds(0);
        if (i < 10)
        {
            var data = new Dictionary<byte, object>();
            data[1] = input + $" {i}!";

            SendData(data, false);
            StartCoroutine(send(input, i + 1));
        }
    }

    byte[] SerializeData(Dictionary<byte, object> data)
    {
        string json = JsonConvert.SerializeObject(data);
        return Encoding.UTF8.GetBytes(json);
    }

    void WriteObject(BinaryWriter writer, object obj)
    {
        Type objType = obj.GetType();

        if (objType == typeof(int))
        {
            writer.Write((int)obj);
        }
        else if (objType == typeof(float))
        {
            writer.Write((float)obj);
        }
        else if (objType == typeof(string))
        {
            writer.Write((string)obj);
        }
        else
        {
            Debug.Log("Unsupported data type: " + objType);
        }
    }
}