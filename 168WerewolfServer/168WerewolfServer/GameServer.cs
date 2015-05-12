using System;
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

using _168WerewolfServer;
/// <summary>
/// Summary description for Class1
/// </summary>
public class GameServer
{
     public static Thread LoginThread;              // Single Thread to handle logins.
     public static Thread LobbyThread;              // Single Thread to handle lobby.   

     public static Queue GameThreads;               // An Queue of GameThreads.

     public static ArrayList Players;               // ArrayList of Players.

	public GameServer()
	{

 
	}



    public static int Main(String[] args) 
    {
        // Initialize the threads with their proper methods.
        LoginThread = new Thread(AsynchronousSocketListener.StartListening);
        LobbyThread = new Thread(AsynchronousSocketListener.LobbyInitialize);

        Console.WriteLine("Login Server Active!");
        LoginThread.Start();

        Console.WriteLine("Lobby Server Active!");
       // LobbyThread.Start();
        
        
        
   
        return 0;
    }

    // A sample Player class that is created.
    public class Player
    {
        String IPEndPoint;
        float positionX;
        float positionY;
        int playerID;

        Player()
        {

        }

        public String getIP()
        {
            return IPEndPoint;
        }

    }

    public void StartGameThread()
    {

    }

}
