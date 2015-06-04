using System;
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

    namespace _168WerewolfServer {
        public class GameAsynchronousSocketListener {

            public string RoomName = "default";                // Empty name 
            public  int GamePort = 0;             // Port is increased, client will know to connect to this.
            // Thread signal.
            public  ManualResetEvent allDoneGame = new ManualResetEvent(false);
              

            public  bool GameStart;

            private  String response = String.Empty;
            public  ArrayList playersInGame;


            public  IPHostEntry gameipHostInfo;
            public  IPAddress gameipAddress;
            public  IPEndPoint gamelocalEndPoint;
            public  IPEndPoint gamelocalEndPointPos;

            public Socket listener;

            // Scoreboard.
            private Scorekeeper sk = new Scorekeeper("SCOREBOARD");

            public GameAsynchronousSocketListener() {

            }

            public GameAsynchronousSocketListener(String n, int i)
            {

                RoomName = n;
                GamePort = i;
                Console.WriteLine("Constructed a new server with name: " + n +  " and port: " + i);

                playersInGame = new ArrayList();


                Thread hb = new Thread(GameHeartbeat);
                hb.Start();

            }

            // "Heartbeat" function
            public void GameHeartbeat()
            {
                while (true)
                {
                    Console.WriteLine("[" + RoomName + "] heartbeat is now active.");

                    // While there are players in the server.
                    while (playersInGame.Count > 0)    
                    {
                        // Encode the game data and send it as a very long string to client.
                        // Fomat: playerID{playerPosX|playerPosY}playerID{playerPosX|playerPosY}
                        String updateS = "[update]";            // Indicates to client that this is an update message.

                        foreach (Player k in playersInGame)
                        {
                            updateS += "*" + k.playerID + "|" + k.positionX + "|" + k.positionY + "|"; // slight edit to ensure playerInGameCount is always viewable
                        }

                        updateS += '~';         // seperation between position update and score updates.

                        //Compile scores into single string
                        string scoreboard = "";
                        ArrayList allScores = sk.GetAllScores();
                        for (int i = 0; i < allScores.Count; i++) // Iterates through all users in database
                        {
                            scoreboard += "*" + ((ArrayList)allScores[i])[0] + "|" + ((ArrayList)allScores[i])[1]; // "*username|score"
                        }

                        updateS += "<EOF>";

                        // Updates all the clients (updates their statuses) and then sleeps. 
                        foreach (Player p in playersInGame)
                        {
                            // Helpful debug statement.
                            Console.WriteLine("[" + RoomName + "] Updating this player: " + p.playerID + " with this: " + updateS);

                            // Invoke the sendgame method, add <EOF> to end of the update string to signal to remote client that it is all.
                           SendGameHeartBeat(p.sock, updateS);
                        }

                        // Sleeps for 100 MS aftr updating all players then continues. 
                        Thread.Sleep(100);

                        Console.WriteLine("Heartbeat!");

                    }

                    Thread.Sleep(100);

                }
            }


            // This allows players to enter the Game, storing their information into an available data structure of players that the game instances can run on.
            public  void StartGameListening() {
                // This is dangerous; make sure to run the Game listener before the status checks.

                Console.WriteLine("Game room: [" + RoomName + "] is now running.");

                // Data buffer for incoming data.
                byte[] bytes = new Byte[1024];

                // Establish the local endpoint for the socket.

                gameipHostInfo = Dns.Resolve(Dns.GetHostName());
                gameipAddress = gameipHostInfo.AddressList[0];
                gamelocalEndPoint = new IPEndPoint(gameipAddress, GamePort);

                // Create a TCP/IP socket.
                listener = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Bind the socket to the local endpoint and listen for incoming connections.
                try {
                    listener.Bind(gamelocalEndPoint);
                    listener.Listen(100);

                    // Handle Game entrances here.
                    while (true) {
                        // Set the event to nonsignaled state.
                        Console.WriteLine("Back to loop a callback; thread has been blocked.");
                       allDoneGame.Reset();


                        //Console.WriteLine("Game room: [" + RoomName + "] is active. :)");

                        // Start an asynchronous socket to listen for connections.
                        Console.WriteLine("[" + RoomName + "] Waiting for Game Data");
                        listener.BeginAccept(new AsyncCallback(AcceptGameCallback), listener);
                        Console.WriteLine("Accepting a callback; thread has been locked.");
                        
                       allDoneGame.WaitOne(); // Wait until a connection is made before continuing.

                    }

                }
                catch (Exception e) {
                    Console.WriteLine(e.ToString());
                }

                Console.WriteLine("\nPress ENTER to continue...");
                Console.Read();

            }

            public  void AcceptGameCallback(IAsyncResult ar) {

                allDoneGame.Set();  // let main thread continue.

                Console.WriteLine("Sets the state of the event to signaled.");

                // Get the socket that handles the client request.
                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);

                // Create the state object.
                GameStateObject state = new GameStateObject();
                state.workSocket = handler;
                handler.BeginReceive(state.buffer, 0, GameStateObject.BufferSize, 0,
                    new AsyncCallback(ReadGameCallback), state);
            }

            public  void ReadGameCallback(IAsyncResult ar) {

                bool TerminatePlayer = false;

                String content = String.Empty;

                // Retrieve the state object and the handler socket
                GameStateObject state = (GameStateObject)ar.AsyncState;
                Socket handler = state.workSocket;

                // Read data from the client socket. // ERRRORING HERE.
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0) {
                    // There  might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Check for end-of-file tag. If it is not there, read more
                    content = state.sb.ToString();
                    if (content.IndexOf("<EOF>") > -1) {
                        // All the data has been read from the client. Display it on the console.
                         Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);

                        // CASE 2: If player gave a "position" update, the game server, game server will update ALL coordinates/situations.
                         if (content.Contains("position")) {
                            // GET THE PLAYER ID from the packet

                            String[] splitted = content.Split('|');

                            int index = int.Parse(splitted[1]);
                            // Code to add here to extract the string
                            // position[450,230]
                            // we also need playerID
                            // apply position updates to this particular player on the server.
                            float posXUpdate = float.Parse(splitted[2]);
                            float posYUpdate = float.Parse(splitted[3]);


                            // Find the player, and update his/her position accordingly!
                            Player e = (Player)playersInGame[index];                // replace the index 
                            e.setPlayerPosition(posXUpdate, posYUpdate);        // set the updates

                            Console.WriteLine("Player set position!");

                        }



                        // CASE 1: If player gave a "joinGame" request, the game server will enqueue the player.
                        else if (content.Contains("joinGame")) {

                            // Create a new player and add him!
                            Player newPlayer = new Player();
                 
                            // Set player info here.                            
                            newPlayer.setPlayerPosition(0.0f, 0.0f);            // Set the initial player position to the center of the map. TODO: Change this to randomly spawn within a certain boundary
                            newPlayer.IPEndPoint = handler.RemoteEndPoint;      // Get the remote endpoint so we can print out helpful debug information.
                            newPlayer.sock = handler;                           // Set the player socket = to the handler.  
                            newPlayer.playerID = playersInGame.Count;           // PID will be set by the number of players.
                            
                            
                            // Add the player to the game.
                            playersInGame.Insert(newPlayer.playerID, newPlayer);     // Player is now added to GameServer and is active!
                            Console.WriteLine("[" + RoomName + "] Player has been added to this game! /n Player ID [" + newPlayer.playerID + "] with IP Endpoint {" + newPlayer.IPEndPoint);

                            string makeString = "[welcome]" + newPlayer.playerID + "|" + playersInGame.Count.ToString()+"<EOF>";

                            Console.WriteLine("Sent this to client: " + makeString);
                            // Send the player a confirmation, along with their ID, total # of players.
                            SendGame(handler, makeString);            // Send the player the ID that they will use to keep track of things.

                        }
                        
      
                         
                        //CASE 4: If player gave a "score" update, the Scoreboard gets updated.
                        else if (content.Contains("score"))
                        {


                            // Leaving this case in here for later implementation.

                            String[] splitted = content.Split('|');

                            string username = splitted[1].ToString();
                            int scoreUpdate = int.Parse(splitted[2]);

                            sk.SetScore(username, scoreUpdate); //Sets the score

                
                            //Send out score to players
                            //SendGame(handler, "score set");
                

                        }

                        //CASE 5: If player gave a "goodbye" update, the player gets removed from the scoreboard and the array of players
                        else if (content.Contains("goodbye"))
                        {
                            String[] splitted = content.Split('|');

                            int playerIndex = int.Parse(splitted[1]);
                            string username = splitted[2];

                            // Removes player from DB
                            sk.RemovePlayer(username);


                            // Not sure if neccesary but I'll leave here for now.

                            Player e = (Player)playersInGame[playerIndex]; //Get disconnecting player 
                            e.setPlayerPosition(-100000, -100000);        // put player waaaay off of the board

                            SendGame(handler, "[disconnection]|"+playerIndex);


                            // Handle the disconnect properly.
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();

                            // Signal to this call that we terminated a player, so we don't try to re-use the handler.
                            TerminatePlayer = true;

                            // remove player from data structure so heartbeat does not affect 
                            playersInGame.Remove(e);


                        }

                         if (!TerminatePlayer)
                         {
                             Console.WriteLine("Finished processing. What now?");


                             GameStateObject newstate = new GameStateObject();
                             newstate.workSocket = handler;

                             handler.BeginReceive(newstate.buffer, 0, GameStateObject.BufferSize, 0,
                             new AsyncCallback(ReadGameCallback), newstate);
                         }
                        

                    }

                }
            }

            private void SendGameHeartBeat(Socket handler, String data)
            {
                // Convert the string data to byte data using ASCII encoding.
                byte[] byteData = Encoding.ASCII.GetBytes(data);

                // Begin sending the data to the remote device.
                handler.BeginSend(Encoding.ASCII.GetBytes(data), 0, Encoding.ASCII.GetBytes(data).Length, 0, null, null);
            }


            private void SendGame(Socket handler, String data) {
                // Convert the string data to byte data using ASCII encoding.
                byte[] byteData = Encoding.ASCII.GetBytes(data);

                Console.WriteLine("Sending this data: " + data);

                // Begin sending the data to the remote device.
                handler.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendGameCallback), handler);



            }

            private  void SendGameCallback(IAsyncResult ar) {
                try {
                    // Retrieve the socket from the state object.
                    Socket handler = (Socket)ar.AsyncState;

                     int bytesRead = handler.EndReceive(ar);

                     Console.WriteLine("Finished sending this data");



                    // Complete sending the data to the remote device.
                   // int bytesSent = handler.EndSend(ar);
       //             Console.WriteLine("[" + RoomName + "] Sent {0} bytes to this client: " + handler.LocalEndPoint, bytesSent);

                    // Do not close handler until client d/c
                   // handler.Shutdown(SocketShutdown.Both);
                    //handler.Close();

                }
                catch (Exception e) {
                    Console.WriteLine(e.ToString());
                }
            }


        }
    }
