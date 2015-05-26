using System;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace _168WerewolfServer
{
    public class Player
    {
        // A sample Player class that is created.

            public EndPoint IPEndPoint;
            public Socket sock;
            public float positionX;
            public float positionY;
            public int playerID;

            public Player() { }

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
    
}
