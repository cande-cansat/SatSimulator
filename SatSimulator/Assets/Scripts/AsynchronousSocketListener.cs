using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
public class StateObject
    {
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
        public Socket workSocket = null;
    }

public class AsynchronousSocketListener
    {
        
        private static ManualResetEvent allDone = new ManualResetEvent(false);
        public AsynchronousSocketListener()
        {
        }

        private static void StartListening()
        {
            // Domain Name System
            // IPHostEntry ipHostInfo = Dns.GetHostEntry("ipAddress");
            // IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 50001);
            Debug.Log("Ip set");

            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    allDone.Reset();
                    Debug.Log("Waiting for a connection....");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Debug.Log((e.ToString()));
            }
            Debug.Log("Enter to continue");
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            Debug.Log("AcceptCallback");
            allDone.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            Debug.Log("Set listener and handler");

            StateObject state = new StateObject();
            state.workSocket = handler;

            Debug.Log("Set StateObject");

            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ReadCallback), state);
        }

        private static void ReadCallback(IAsyncResult ar)
        {
            // Debug.Log("ReadCallback");
            String content = String.Empty;

            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead = handler.EndReceive(ar);
            

            if (bytesRead > 0)
            {
                // 데이터가 더 있을 수도 있기 때문에, 받은 데이터를 저장함
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // 모든 데이터를 읽어왔음
                    Debug.Log(content);

                    // Echo to client
                    
                    Send(handler, content);
                }
                else
                {
                    // 아직 읽을 데이터가 남았기에 마저 Receive
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private static void Send(Socket handler, String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            handler.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;

                int bytesSent = handler.EndSend(ar);
                Debug.LogFormat("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        public void ActivateServer()
        {
            
            Thread thread = new Thread(new ThreadStart(StartListening));
            thread.IsBackground = true;
            thread.Start();
        }
    }