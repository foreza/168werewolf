using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

class GameHandler
{

}

// State object for reading client data asynchronously

    public class GameStateObject
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

    // A sample Player class that is created.
    public class Player
    {
        String IPEndPoint;
        float positionX;
        float positionY;
        int playerID;

        Player()
        {}

        public String getIP()
        {
            return IPEndPoint;
        }

        public float getX()
        {
            return positionX;
        }

        public float getY()
        {
            return positionY;
        }

        public int getPID()
        {
            return playerID;
        }

        public void setPlayerPosition(float x, float y)
        {
            positionX = x;
            positionY = y;
        }


    }


public class GameAsynchronousSocketListener
{

    public static int GamePort = 11002;             // Port is increased, client will know to connect to this.
    // Thread signal.
    public static ManualResetEvent allDoneGame = new ManualResetEvent(false);
    public static ManualResetEvent GameMessageRound = new ManualResetEvent(false);


    public static Queue<String> playersInGame;

    public GameAsynchronousSocketListener()
    {

    }
    /*
    public static void StartGameStatusCheck()
    {

        while (true)
        {
            int playersInGameCount = playersInGame.Count;

            Console.WriteLine("Current # of players in Game: [" + playersInGameCount + "] Welcome! ");

            while (playersInGameCount > 0)
            {
                String p = playersInGame.Peek();
                Console.WriteLine("Player with endpoint IP: " + p);

                playersInGameCount--;
                playersInGame.Enqueue(p);
                playersInGame.Dequeue();
            }


            // Wait for next player to enter..
            GameMessageRound.WaitOne(5000);
        }

    }

     */

    // This allows players to enter the Game, storing their information into an available data structure of players that the game instances can run on.
    public static void StartGameListening()
    {
        // This is dangerous; make sure to run the Game listener before the status checks.
        playersInGame = new Queue<String>();

        Console.WriteLine("Game is now running.");

        // Data buffer for incoming data.
        byte[] bytes = new Byte[1024];

        // Establish the local endpoint for the socket.
        // The DNS name of the computer
        // running the listener is "host.contoso.com".
        IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, GamePort);

        // Create a TCP/IP socket.
        Socket listener = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the local endpoint and listen for incoming connections.
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(100);

            // Handle Game entrances here.
            while (true)
            {
                // Set the event to nonsignaled state.
                allDoneGame.Reset();

                // Start an asynchronous socket to listen for connections.
                Console.WriteLine("Waiting for Game Data");
                listener.BeginAccept(new AsyncCallback(AcceptGameCallback), listener);
                // Wait until a connection is made before continuing.
                allDoneGame.WaitOne();

                // Allow Gamemessages to be sent out to update the Game list of "available" players.


            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        Console.WriteLine("\nPress ENTER to continue...");
        Console.Read();

    }

    public static void AcceptGameCallback(IAsyncResult ar)
    {
        // Signal the main thread to continue.
        allDoneGame.Set();

        // Get the socket that handles the client request.
        Socket listener = (Socket)ar.AsyncState;
        Socket handler = listener.EndAccept(ar);

        Console.WriteLine("Obtaining Game Data from" + handler.LocalEndPoint);

        // Create the state object.
        GameStateObject state = new GameStateObject();
        state.workSocket = handler;
        handler.BeginReceive(state.buffer, 0, GameStateObject.BufferSize, 0,
            new AsyncCallback(ReadGameCallback), state);
    }

    public static void ReadGameCallback(IAsyncResult ar)
    {
        String content = String.Empty;

        // Retrieve the state object and the handler socket
        // from the asynchronous state object.
        GameStateObject state = (GameStateObject)ar.AsyncState;
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

                // CASE 1: If player gave a "joinGame" request, the game server will enqueue the player.
                if (content.Contains("joinGame"))
                {
                    playersInGame.Enqueue(handler.LocalEndPoint.ToString());        // Player is now added to GameServer and is active!
                    Console.WriteLine("Player added to Game");
                }
                // CASE 2: If player gave a "position" update, the game server, game server will update all coordinates/situations.
                if(content.Contains("position"))
                {
                    // Code to add here to extract the string
                    // position[450,230]
                    // we also need playerID
                    // apply position updates to the game server.
                    Console.WriteLine("Server applied this position update to this player: " + content);
                }


                // Encode the game data and send it as a very long string to client.
                // Example: 

                SendGame(handler, content);
            }
            else
            {
                // Not all data received. Get more.
                handler.BeginReceive(state.buffer, 0, GameStateObject.BufferSize, 0,
                new AsyncCallback(ReadGameCallback), state);
            }
        }
    }

    private static void SendGame(Socket handler, String data)
    {
        // Convert the string data to byte data using ASCII encoding.
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to the remote device.
        handler.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendGameCallback), handler);
    }

    private static void SendGameCallback(IAsyncResult ar)
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
