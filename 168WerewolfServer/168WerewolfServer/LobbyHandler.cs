
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;




    class LobbyHandler
    {

    }

    // State object for reading client data asynchronously
/*
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
 * */
    public class LobbyAsynchronousSocketListener
{

    public static int lobbyPort = 11001;             // Port is increased by 1, client will know to connect to this.
    // Thread signal.
    public static ManualResetEvent allDoneLobby = new ManualResetEvent(false);

    public static ManualResetEvent lobbyMessageRound = new ManualResetEvent(false);


    public static Queue<String> playersInLobby;

    public LobbyAsynchronousSocketListener()
    {
       
    }

    public static void StartLobbyStatusCheck()
    {

        while (true)
        {
            int playersInLobbyCount = playersInLobby.Count;

            Console.WriteLine("Current # of players in lobby: [" + playersInLobbyCount + "] Welcome! ");

            while (playersInLobbyCount > 0)
            {
                String p = playersInLobby.Peek();
                Console.WriteLine("Player with endpoint IP: " + p);

                playersInLobbyCount--;
                playersInLobby.Enqueue(p);
                playersInLobby.Dequeue();
            }


            // Wait for next player to enter..
            lobbyMessageRound.WaitOne(5000);
        }

    }
        

    // This allows players to enter the lobby, storing their information into an available data structure of players that the game instances can run on.
    public static void StartLobbyListening()
    {
        // This is dangerous; make sure to run the lobby listener before the status checks.
        playersInLobby = new Queue<String>();

        Console.WriteLine("Lobby is now running.");

        // Data buffer for incoming data.
        byte[] bytes = new Byte[1024];

        // Establish the local endpoint for the socket.
        // The DNS name of the computer
        // running the listener is "host.contoso.com".
        IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, lobbyPort);

        // Create a TCP/IP socket.
        Socket listener = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the local endpoint and listen for incoming connections.
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(100);

            // Handle Lobby entrances here.
            while (true)
            {
                // Set the event to nonsignaled state.
                allDoneLobby.Reset();

                // Start an asynchronous socket to listen for connections.
                Console.WriteLine("Waiting for people to join...");
                listener.BeginAccept(new AsyncCallback(AcceptLobbyCallback), listener);

                // Wait until a connection is made before continuing.
                allDoneLobby.WaitOne();

                // Allow lobbymessages to be sent out to update the lobby list of "available" players.


            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        Console.WriteLine("\nPress ENTER to continue...");
        Console.Read();

    }

    public static void AcceptLobbyCallback(IAsyncResult ar)
    {
        // Signal the main thread to continue.
        allDoneLobby.Set();

        // Get the socket that handles the client request.
        Socket listener = (Socket)ar.AsyncState;
        Socket handler = listener.EndAccept(ar);

        Console.WriteLine("Accepting client lobby request: " + handler.LocalEndPoint);

        // Create the state object.
        StateObject state = new StateObject();
        state.workSocket = handler;
        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
            new AsyncCallback(ReadLobbyCallback), state);
    }

    public static void ReadLobbyCallback(IAsyncResult ar)
    {
        String content = String.Empty;

        // Retrieve the state object and the handler socket
        // from the asynchronous state object.
        StateObject state = (StateObject)ar.AsyncState;
        Socket handler = state.workSocket;

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
                Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                    content.Length, content);

                // CASE 1: If player gave a "joinLobby" request, the server will enqueue the player.
                if(content.Contains("joinLobby"))
                {
                    playersInLobby.Enqueue(handler.LocalEndPoint.ToString());
                    Console.WriteLine("Player added to lobby");
                }
                // CASE 2: If player gave a "joinGame" request, the server will attempt to place player in active game session
                if (content.Contains("joinGame"))
                {
                    Console.WriteLine("Player: " + handler.LocalEndPoint.ToString() + " is now joining game instance...");
                }

                // Add the player into the data structure.

                
                SendLobby(handler, content);
            }
            else
            {
                // Not all data received. Get more.
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadLobbyCallback), state);
            }
        }
    }

    private static void SendLobby(Socket handler, String data)
    {
        // Convert the string data to byte data using ASCII encoding.
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to the remote device.
        handler.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendLobbyCallback), handler);
    }

    private static void SendLobbyCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            Socket handler = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.
            int bytesSent = handler.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to this client: " + handler.LocalEndPoint, bytesSent);

          handler.Shutdown(SocketShutdown.Both);
          handler.Close();

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }


}
