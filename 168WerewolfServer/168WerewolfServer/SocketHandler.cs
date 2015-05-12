﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace _168WerewolfServer
{


    // https://msdn.microsoft.com/en-us/vstudio/aa496123
    // Below Code is from https://msdn.microsoft.com/en-us/library/fx6588te%28v=vs.110%29.aspx
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    // State object for reading client data asynchronously
    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class AsynchronousSocketListener
    {

        public static Socket listener;              // The endpoint listener on the server.
        public static Queue endPoints;              //  

        public AsynchronousSocketListener()
        {

        }


        static public string HandleInput(string data) {
            string response;
            string[] inputs = data.Split(':');
            try {
                if (inputs[0] == "<login>") {
                    LoginHandler lc = new LoginHandler();
                    string[] loginPackage = new string[] { inputs[1], inputs[2] }; //JASON! USE JSON! loginpackage = {'username':un,'password':pw}
                    //Determines whether login is correct.
                    Console.WriteLine("Accessing Database for login");
                    lc.StartDatabase();
                    response = lc.AccessDB(loginPackage);
                    lc.CloseDatabase();
                }
                else {
                    Console.WriteLine(data);
                    response = data;
                }
            }
            catch (IndexOutOfRangeException e) {
                Console.WriteLine("Indexing was messed up somehow. Did you put the wrong prefix on?");
                response = "error";
            }
            return response;
        }
        
        public static void LobbyInitialize()
        {
            Console.WriteLine("Initialized lobby.");
            endPoints = new Queue();
            allDone.WaitOne(2000);

            while(true)
            {

                int lobbySize = endPoints.Count;

                Console.WriteLine("Players in lobby: " + lobbySize);
                allDone.WaitOne(1000);


                if (lobbySize > 0)            // If there are endpoints to process
                {
                    Socket thePlayerEP = (Socket)endPoints.Dequeue();
                    Console.WriteLine("Attempting to send message to this player with EP: " + thePlayerEP.LocalEndPoint);
                    Send(thePlayerEP, "Currently in Lobby<EOF>");
                    Console.WriteLine("Finished sending message to this player with EP: " + thePlayerEP.LocalEndPoint);

                    endPoints.Enqueue(thePlayerEP);

                    lobbySize--;

                    Console.WriteLine("Moving into next player (if existing.");

                }

                Console.WriteLine("Processed all in queue. Restarting check...");
                allDone.WaitOne(1000);



            }

        }
        
        
        // Thread signal.
        public static ManualResetEvent allDone = new ManualResetEvent(false);

    
        public static void StartListening()
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.
            // The DNS name of the computer
            // running the listener is "host.contoso.com".
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress,9999);

            // Create a TCP/IP socket.
            listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept( new AsyncCallback(AcceptCallback), listener);

                    // Wait 5s for a connection is made before continuing.
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            Console.WriteLine("Receiving data!");

            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read 
                // more data.
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the 
                    // client. Display it on the console.
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);

                    //Decide what to send back based on what was received
                    string response = HandleInput(content);

                    Console.WriteLine("Current response: " + response);

                    // Send designated response.
                    Send(handler, response);

                    Console.WriteLine("Log in check deny??? " + response);

                    if(response.Contains("Accepted"))
                    {
                        Console.WriteLine("Putting this player into the Lobby.");
                        endPoints.Enqueue(handler);                                     // Places the player's endpoint into the queue for message sending.
                    }
                }
                else
                {
                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }


        private static void Send(Socket handler, String data)
        {
            Console.WriteLine("Sending: " + data);
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


      
    }

}
