using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;

public class SocketServer : MonoBehaviour
{

    private Socket m_Server, m_Client;
    public int m_Port = 50001;
    public ToServerPacket m_ReceivePacket = new ToServerPacket();
    private EndPoint m_RemoteEndPoint;
    private TcpListener tcpListener;
    private Thread tcpListenerThread;
    private TcpClient connectedTcpClient;
    private static SocketServer instance = null;

    void Awake()
    {
        Debug.Log("Start Server");
        instance = this;

    }
    void Start()
    {
        InitServer();
    }

    void Update()
    {
        Receive();
        // Send();
    }

    void OnApplicationQuit()
    {
        CloseServer();
    }

    void InitServer()
    {
        m_Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_RemoteEndPoint = new IPEndPoint(IPAddress.Any, m_Port);
        m_Server.Bind(m_RemoteEndPoint);
        m_Server.Listen(10); // client 접속을 기다림.
        m_Client = m_Server.Accept(); // client가 하나만 사용.
    }

    void Receive()
    {
        int receive = 0;
        byte[] packet = new byte[1024];

        try
        {
            receive = m_Client.Receive(packet);
        }

        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
            return;
        }

        m_ReceivePacket = ByteArrayToStruct<ToServerPacket>(packet);

        if (receive > 0)
        {
            DoReceivePacket(); // 받은 값 처리
        }
    }

    void DoReceivePacket()
    {
        Debug.LogFormat($"sat_x = {m_ReceivePacket.sat_x} " +
            $"sat_y = {m_ReceivePacket.sat_y } " +
            $"sat_z = {m_ReceivePacket.sat_z} " +
            $"target_x = {m_ReceivePacket.target_x}" +
            $"target_y = {m_ReceivePacket.target_y} " +
            $"target_z = {m_ReceivePacket.target_z} ");
        
    }


    void CloseServer()
    {
        if (m_Client != null)
        {
            m_Client.Close();
            m_Client = null;
        }

        if (m_Server != null)
        {
            m_Server.Close();
            m_Server = null;
        }
    }

    T ByteArrayToStruct<T>(byte[] buffer) where T : struct
    {
        int size = Marshal.SizeOf(typeof(T));
        if (size > buffer.Length)
        {
            throw new Exception();
        }

        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(buffer, 0, ptr, size);
        T obj = (T)Marshal.PtrToStructure(ptr, typeof(T));
        Marshal.FreeHGlobal(ptr);
        return obj;
    }
    
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Serializable]
public struct ToServerPacket
{
    
    public float sat_x;
    public float sat_y;
    public float sat_z;

    public float target_x;
    public float target_y;
    public float target_z;
}