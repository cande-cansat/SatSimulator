using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


public delegate void CallbackGpsPosition(float sat_x, float sat_y, float sat_z, float target_x, float target_y, float target_z);
    
public class TCP_Server : MonoBehaviour
{
    #region private members
    private TcpListener tcpListener;
    private Thread tcpListenerThread;
    private TcpClient connectedTcpClient;
    #endregion

    private static TCP_Server instance = null;
    CallbackGpsPosition callbackGpsPosition;

    void Awake()
    {
        Debug.Log("Start Server");
        instance = this;

        // Start TcpServer background thread
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequest));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static TCP_Server Instance
    {
        get
        {
            if(instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    public void SetGpsPositionCallback(CallbackGpsPosition callback)
    {
        if(callbackGpsPosition == null)
        {
            callbackGpsPosition = callback;
        } else
        {
            callbackGpsPosition += callback;
        }
    }

    // Runs in background TcpServerThread; Handles incomming TcpClient requests
    private void ListenForIncommingRequest()
    {
        try
        {
            // Create listener on 192.168.0.2 port 50001
            tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 50003);
            tcpListener.Start();
            Debug.Log("Server is listening");

            while (true)
            {
                using (connectedTcpClient = tcpListener.AcceptTcpClient())
                {
                    // Get a stream object for reading
                    using (NetworkStream stream = connectedTcpClient.GetStream())
                    {
                        // Read incomming stream into byte array.
                        do
                        {
                            Byte[] sat_x = new Byte[4];
                            Byte[] sat_y = new Byte[4];
                            Byte[] sat_z = new Byte[4];
                            Byte[] target_x = new Byte[4];
                            Byte[] target_y = new Byte[4];
                            Byte[] target_z = new Byte[4];
                            

                            Byte[] bytes = new Byte[24];
                            int length = stream.Read(bytes, 0, 24);

                            HandleIncommingRequest(24, bytes);
                        } while (true);
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }
    }

    // Handle incomming request
    private void HandleIncommingRequest(int payloadLength, byte[] bytes)
    {
        GetPositionHandler(bytes);
    }

    void OnApplicationQuit()
    {
        AbortThread();
    }

    public void AbortThread()
    {
        tcpListenerThread.Abort();
    }


    // Handle Touch Signal
    private void GetPositionHandler(byte[] bytes)
    {
        Debug.Log("Execute Gps Position Handler");
        float val_sat_x = BitConverter.ToSingle(bytes, 0);
        float val_sat_y = BitConverter.ToSingle(bytes, 4);
        float val_sat_z = BitConverter.ToSingle(bytes, 8);
        
        float val_target_x = BitConverter.ToSingle(bytes, 12);
        float val_target_y = BitConverter.ToSingle(bytes, 16);
        float val_target_z = BitConverter.ToSingle(bytes, 20);


        Debug.Log("val_sat_x     : " + val_sat_x);
        Debug.Log("val_sat_y     : " + val_sat_y);
        Debug.Log("val_sat_y     : " + val_sat_z);
        
        Debug.Log("val_target_x     : " + val_target_x);
        Debug.Log("val_target_y     : " + val_target_y);
        Debug.Log("val_target_z     : " + val_target_z);

        if(callbackGpsPosition != null)
        {
            callbackGpsPosition(val_sat_x, val_sat_y, val_sat_z, val_target_x, val_target_y, val_target_z);
        }
    }
}