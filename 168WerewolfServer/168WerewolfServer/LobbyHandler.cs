
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using _168WerewolfServer;


// 128.195.21.135
    class LobbyHandler
    {

    }

    public class LobbyAsynchronousSocketListener
{

    public static int lobbyPort = 11001;             // Port is increased by 1, client will know to connect to this.
    public static int gamePorts = 11002;            // GamePorts start from 11002.

    public static Queue<GameThread> RunningGameInstances;       // Store running game instances here once you make them.
    // Thread signal.
    public static ManualResetEvent allDoneLobby = new ManualResetEvent(false);

    public static ManualResetEvent lobbyMessageRound = new ManualResetEvent(false);


    public struct PlayerLobbyObj
    {
        public String theEndPoint;
        public int gamePortNumber;
    }

    public struct GameThread
    {
        public Thread t;            // The thread object
        public String name;         // Name of Game room
        public int portNumber;      // assigned port iD
    }

    public static Queue<PlayerLobbyObj> playersInLobby;

    public LobbyAsynchronousSocketListener()
    {
  
    }


    // Call this function to begin another thread.
    public static void StartNewGameThread(string name, int instance)
    {

        // Create a GameThread object
        GameThread gt = new GameThread();
        gt.name = name;
        gt.portNumber = GetInstancePortNumber(instance);   // change
        GameAsynchronousSocketListener g = new GameAsynchronousSocketListener(name, gt.portNumber);                // Create a new socket listener for a game server
        Thread NewGameThread = new Thread(g.StartGameListening);                                        // define instantiate it.
        gt.t = NewGameThread;
        NewGameThread.Start();                                                                          // Start the instance.

        // Enqueue the instance.
        RunningGameInstances.Enqueue(gt);
        Console.WriteLine("Added a new game thread with name: " + gt.name + "and listening to port: " + gt.portNumber);

    }

    // This function returns a port number based off of the initial port number.
    // We may do something with the param we pass in.
    public static int GetInstancePortNumber(int i)
    {
        return (gamePorts + RunningGameInstances.Count);

    }


    public static void StartLobbyStatusCheck()
    {

        while (true && playersInLobby != null)
        {
            int playersInLobbyCount = playersInLobby.Count;

            Console.WriteLine("Current # of players in lobby: [" + playersInLobbyCount + "] Welcome! ");

            while (playersInLobbyCount > 0)
            {
                String p = playersInLobby.Peek().theEndPoint;
                Console.WriteLine("Player with endpoint IP: " + p);

                playersInLobbyCount--;

                playersInLobby.Enqueue(playersInLobby.Peek());
                playersInLobby.Dequeue();
            }


            // Wait for next player to enter..
            lobbyMessageRound.WaitOne(5000);
        }

    }

      public static bool CheckGameExists(string n)
    {
        // A quick method that checks through the entire queue of current game threads.
 
        int size = RunningGameInstances.Count;
        for (int i = 0; i < size; ++i)
        {
            // Compare names.
            if (n.Equals(RunningGameInstances.Peek().name))
            {
                // If any of them are true
                return true;
            }
            RunningGameInstances.Enqueue(RunningGameInstances.Peek());
            RunningGameInstances.Dequeue();
        }
          // Game does not exist! Signal to main caller to make a new one.
            return false;
    }
        

    // This allows players to enter the lobby, storing their information into an available data structure of players that the game instances can run on.
    public static void StartLobbyListening()
    {
        playersInLobby = new Queue<PlayerLobbyObj>();
        RunningGameInstances = new Queue<GameThread>();

        Console.WriteLine("Lobby is now running.");

        Console.WriteLine("Testing game server functionality...");

        StartNewGameThread("Game1", 1);
        StartNewGameThread("Game2", 1);
        StartNewGameThread("Game3", 1);
        StartNewGameThread("Game4", 1);


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
                    // CHANGE THIS SO WE ENQUEUE PLAYERS.
                    // a struct.

                    // We will recieve the name of the game that they want to join.

                    // TODO: Parse it, and check the name.

                    PlayerLobbyObj temp = new PlayerLobbyObj();
                    temp.theEndPoint = handler.LocalEndPoint.ToString();

                    playersInLobby.Enqueue(temp);

                    // TODO: Do we need to start a new game instance?
                    // Start the game instance server once the player logs in - once they hit play, they will be live.
                    // Basically, first person to login creates server

                    // Check if we need to
                    if (CheckGameExists("temp"))
                    {
                        // Set the player object's port to that number
                        // Player will then connect
                    }

                    else
                    {
                        // Create a new game instance with that number
                        // Make a new game port as well
                        // Set the player obj port to that number
                        // Run game instance, and player can then connnect to it.
                        

                    }

                    Console.WriteLine("Player added to lobby.");
                    SendLobby(handler, "welcomeToLobby");
                }
                // CASE 2: If player gave a "joinGame" request, the server will attempt to place player in active game session
                else if (content.Contains("joinGame"))
                {
                    Console.WriteLine("Player: " + handler.LocalEndPoint.ToString() + " is now joining game instance...");
                    SendLobby(handler, "startGame");
                }

                // Add the player into the data structure.

                else
                { 
                SendLobby(handler, content);
                }
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
