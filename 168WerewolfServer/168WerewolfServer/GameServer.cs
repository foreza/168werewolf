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
     public static Thread LobbyCheckThread;         // Single Thread to handle lobby statuses/debug/print.


     //public static Queue GameThreads;               // An Queue of GameThreads. Currently Unused

    // public static Thread GameThread;                     // Temporary single instance of a game thread.

	public GameServer()
	{
       // Players = new ArrayList();
    
	}



    public static int Main(String[] args) 
    {
        // Initialize the threads with their proper methods.
        LoginThread = new Thread(AsynchronousSocketListener.StartListening);
        LobbyThread = new Thread(LobbyAsynchronousSocketListener.StartLobbyListening);
        LobbyCheckThread = new Thread(LobbyAsynchronousSocketListener.StartLobbyStatusCheck);
        
        // Since we changed the methods from static -> non static, we may have to do it this way!
        //GameAsynchronousSocketListener game = new GameAsynchronousSocketListener();
 
        //GameThread = new Thread(game.StartGameListening);
       // GamePositionThread = new Thread(GameAsynchronousSocketListener.SendPositionUpdates);

        Console.WriteLine("Login Server Active!");
        LoginThread.Start();

        Console.WriteLine("Lobby Server Active!");
        LobbyThread.Start();
        LobbyCheckThread.Start();

        Console.WriteLine("Wating for player connections. Game servers will initialize upon login!");

        /* Do not have main loop start a game server! */
        //Console.WriteLine("Initializing a temporary Game Server!");
        //GameThread.Start();
        //GamePositionThread.Start();
   
        return 0;
    }

   

}
