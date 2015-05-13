﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;
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


public class GameAsynchronousSocketListener
{

    public static int GamePort = 11002;             // Port is increased, client will know to connect to this.
    public static int GamePort2 = 11003;     
    // Thread signal.
    public static ManualResetEvent allDoneGame = new ManualResetEvent(false);
    public static ManualResetEvent GameMessageRound = new ManualResetEvent(false);

    public static bool GameStart;

    private static String response = String.Empty;


    // A sample Player class that is created.
    public class Player
    {
        public EndPoint IPEndPoint;
        public Socket sock;
        public float positionX;
        public float positionY;
        public int playerID;

        public Player()
        { }

        public Socket getSock()
        {
            return sock;
        }

        public EndPoint getIP()
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


    public static ArrayList playersInGame;


    public static IPHostEntry gameipHostInfo;
    public static IPAddress gameipAddress;
    public static IPEndPoint gamelocalEndPoint;
    public static IPEndPoint gamelocalEndPointPos;

    private static ManualResetEvent gameconnectDone =
    new ManualResetEvent(false);
    private static ManualResetEvent gamesendDone =
        new ManualResetEvent(false);
    private static ManualResetEvent gamereceiveDone =
        new ManualResetEvent(false);

    public GameAsynchronousSocketListener()
    {

    }

    public static void SendPositionUpdates()
    {
        // This loop will constantly send out position updates to all connected clients.
        /*
        while(true)
        {

            // Console.WriteLine("HIHI" + GameStart);
            // This will not run until the game has started.
            if(playersInGame != null && playersInGame.Count != 0)
            {
                Console.WriteLine("Begin position updates!");

                String updateString = "testStringUpdate";
                // Convert the string data to byte data using ASCII encoding.
                byte[] byteData = Encoding.ASCII.GetBytes(updateString);


                ArrayList iterable = playersInGame;     // Make a new temporary arraylist.

                for (int i = 0; i < iterable.Count; i++)
                {
                    Player p = (Player)iterable[i];     // Store player in temporary variable.
                    Socket s = p.getSock();             // Gets the socket of the player. THIS MAY NOT WORK AND WE MAY GO BACK TO THE DRAWING BOARD.
                    // REPLACE W/ POSITION DATA
                    //Console.WriteLine("Sending this player the position update: " + s.RemoteEndPoint);
                    //SendGame(s, "position data put here");
                    //Console.WriteLine("Updating this player: " + p.getPID() + " ");
                    GameStateObject state = new GameStateObject();

                    s.BeginConnect(p.IPEndPoint, new AsyncCallback(GameConnectCallback), s);
                    gameconnectDone.WaitOne();
                    Console.WriteLine("Connected. Sending data to player: " + p.getPID());
                    SendGame(s, updateString + "<EOF>");
                    gamesendDone.WaitOne(1000);
                    gameReceive(s);
                    gamereceiveDone.WaitOne(1000);
                   // s.BeginSendTo(byteData, 0, byteData.Length, 0, p.IPEndPoint, new AsyncCallback(SendGameCallback), state);


                }

            }
        }
         * */
    }

    private static void GameConnectCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            Socket client = (Socket)ar.AsyncState;

            // Complete the connection.
            client.EndConnect(ar);

            Console.WriteLine("Socket connected to {0}" + client.RemoteEndPoint.ToString());

            // Signal that the connection has been made.
            gameconnectDone.Set();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private static void gameReceive(Socket client)
    {
        try
        {
            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = client;

            // Begin receiving the data from the remote device.
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                                new AsyncCallback(gameReceiveCallback), state);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private static void gameReceiveCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the state object and the client socket 
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket client = state.workSocket;

            // Read data from the remote device.
            int bytesRead = client.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There might be more data, so store the data received so far.
                state.sb.Append(Encoding.Unicode.GetString(state.buffer, 0, bytesRead));

                // Get the rest of the data.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                                    new AsyncCallback(gameReceiveCallback), state);
            }
            else
            {
                // All the data has arrived; put it in response.
                if (state.sb.Length > 1)
                {
                    response = state.sb.ToString();
                }
                // Signal that all bytes have been received.
                gamereceiveDone.Set();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    // This allows players to enter the Game, storing their information into an available data structure of players that the game instances can run on.
    public static void StartGameListening()
    {
        // This is dangerous; make sure to run the Game listener before the status checks.
        playersInGame = new ArrayList();

        Console.WriteLine("Game is now running.");

        // Data buffer for incoming data.
        byte[] bytes = new Byte[1024];

        // Establish the local endpoint for the socket.
        // The DNS name of the computer
        // running the listener is "host.contoso.com".
        gameipHostInfo = Dns.Resolve(Dns.GetHostName());
        gameipAddress = gameipHostInfo.AddressList[0];
        gamelocalEndPoint = new IPEndPoint(gameipAddress, GamePort);
        gamelocalEndPointPos = new IPEndPoint(gameipAddress, GamePort2);

        // Create a TCP/IP socket.
        Socket listener = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the local endpoint and listen for incoming connections.
        try
        {
            listener.Bind(gamelocalEndPoint);
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

                    Player newPlayer = new Player();
                    Socket newSock = new Socket(AddressFamily.InterNetwork,
                                           SocketType.Stream, ProtocolType.Tcp);

                    newPlayer.setPlayerPosition(0.0f, 0.0f);
                    newPlayer.IPEndPoint = handler.RemoteEndPoint;
                    newPlayer.sock = newSock;
                    newPlayer.playerID = playersInGame.Count; // PID will be set by the number of players.
                    playersInGame.Insert(newPlayer.playerID, newPlayer);     // Player is now added to GameServer and is active!
                    Console.WriteLine("Player has been added to Game! Player ID[" + newPlayer.playerID + "with IP Endpoint {" + newPlayer.IPEndPoint );

                    SendGame(handler, "[welcome]" + newPlayer.playerID);            // Send the player the ID that they will use to keep track of things.

                }
                // CASE 2: If player gave a "position" update, the game server, game server will update ALL coordinates/situations.
                else if(content.Contains("position"))
                {
                    // GET THE PLAYER ID from the packet

                    String[] splitted = content.Split('|');

                   

                    int index = int.Parse(splitted[1]);
                    // Code to add here to extract the string
                    // position[450,230]
                    // we also need playerID
                    // apply position updates to this particular player on the server.
                    float posXUpdate = float.Parse(splitted[2]);
                    float posYUpdate = float.Parse(splitted[3]);

                    Player e = (Player)playersInGame[index];                // replace the index 
                    e.setPlayerPosition(posXUpdate, posYUpdate);        // set the updates
                    Console.WriteLine("Server applied this position update to player: " + index + "content: " + content);


                    // Encode the game data and send it as a very long string to client.
                    // Example: 

                    // Fomat: playerID{playerPosX|playerPosY}playerID{playerPosX|playerPosY}
                    String updateS = "";

                    for (int i = 0; i < playersInGame.Count; i++)
                    {
                         Player k = (Player)playersInGame[i];
                         updateS += k.playerID + "{" + k.positionX + "|" + k.positionY + "}";
                    }

                    SendGame(handler, "[update]" + updateS);
                }




               
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
