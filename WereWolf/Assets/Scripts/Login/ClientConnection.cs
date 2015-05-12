using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


public class ClientConnection : MonoBehaviour {

		public static string userDisplayName;
		private const int port = 11000;						// The port number for the remote device.
		// private static bool connectToServer = false;	
		public static string address = "169.234.59.242";
        private Socket client;
		
	// ManualResetEvent instances signal completion.
	private static ManualResetEvent connectDone = 
		new ManualResetEvent(false);
	private static ManualResetEvent sendDone = 
		new ManualResetEvent(false);
	private static ManualResetEvent receiveDone = 
		new ManualResetEvent(false);
		// The response from the remote device.
		private static String response = "";

        GameObject responseHandler;

        void Start() {
            responseHandler = GameObject.Find("ResponseHandler");
        }

	IEnumerator StartClient(string[] LoginPackage) {
			// Connect to a remote device.
			try {
                address = LoginPackage[2];
				// Establish the remote endpoint for the socket.
				// The name of the 
				// remote device is "host.contoso.com".
			// This is hard coded in for now.

			//string address = "174.77.35.116"; //Jason's hardcoded IP
            //string address = "169.234.54.128"; //Connor's hardcoded IP
			print ("Starting connection. Connection: " + address);
		
			IPHostEntry ipHostInfo = Dns.GetHostEntry(address);
			// print ("Starting connection. connection: " + iNeedanAddress);

			// This is where the client will hang if it can't get the DNS host entry.
			//TODO: Have a way for the client to "safe fail" out of the state.

				// IPHostEntry ipHostInfo = Dns.Resolve("host.contoso.com");
				IPAddress ipAddress = ipHostInfo.AddressList[0];
				IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

				// Create a TCP/IP socket.
				client = new Socket(AddressFamily.InterNetwork,
				                           SocketType.Stream, ProtocolType.Tcp);
				

				// Connect to the remote endpoint.
				client.BeginConnect( remoteEP, 
				                    new AsyncCallback(ConnectCallback), client);
				connectDone.WaitOne(5000);

				
				// Send test data to the remote device.
                Send(client, "<login>:" + LoginPackage[0] + ":" + LoginPackage[1] + ":<EOF>");
			//Send(client,"Player [" + userDisplayName + "] says: This is a test! Hello, server.:<EOF>");

			// YOU NEED TO END IT WITH <EOF> OMG.
			//Send(client,"This is a test<EOF>");
				sendDone.WaitOne(5000);
				
				// Receive the response from the remote device.
				Receive(client);
				receiveDone.WaitOne(5000);
				
				// Write the response to the console.
				print("Response received : {0}" + response.ToString());
                // Handle the response
                responseHandler.SendMessage("HandleResponse", response);

				
			} catch (Exception e) {
				print(e.ToString());
			print ("FAILED TO CONNECT");
			Application.Quit();

			}

		yield return new WaitForSeconds(1);
	}

    IEnumerator CloseClient() {
        // Connect to a remote device.
        try {
            // Release the socket.
            print("Closing client connection...");
            Send(client, "Goodbye!<EOF>");
            client.Shutdown(SocketShutdown.Both);
            client.Close();

            print("Client closed connection");

        }
        catch (Exception e) {
            print(e.ToString());
            print("FAILED TO CLOSE");
            Application.Quit();

        }

        yield return new WaitForSeconds(1);
    }
		
		private static void ConnectCallback(IAsyncResult ar) {
			try {
				// Retrieve the socket from the state object.
				Socket client = (Socket) ar.AsyncState;
				
				// Complete the connection.
				client.EndConnect(ar);
				
				print("Socket connected to {0}" + 
				                  client.RemoteEndPoint.ToString());
				
				// Signal that the connection has been made.
				connectDone.Set();
                print("Connected to server! Welcome.");
			} catch (Exception e) {
				print(e.ToString());
			print ("FAILED IN CONNECTCALLBACK");
			Application.Quit();

			}
		}
		
		private static void Receive(Socket client) {
			try {
				// Create the state object.
				StateObject state = new StateObject();
				state.workSocket = client;
				
				// Begin receiving the data from the remote device.
				client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,
				                    new AsyncCallback(ReceiveCallback), state);
			} catch (Exception e) {
				print(e.ToString());
			print ("FAILED TO RECEIVE");
			Application.Quit();
			}
		}
		
		private static void ReceiveCallback( IAsyncResult ar ) {
			try {
				// Retrieve the state object and the client socket 
				// from the asynchronous state object.
				StateObject state = (StateObject) ar.AsyncState;
				Socket client = state.workSocket;
				
				// Read data from the remote device.
				int bytesRead = client.EndReceive(ar);

				if (bytesRead > 0) {
					// There might be more data, so store the data received so far.
					state.sb.Append(Encoding.Unicode.GetString(state.buffer,0,bytesRead));
					
					// Get the rest of the data.
					client.BeginReceive(state.buffer,0,StateObject.BufferSize,0,
					                    new AsyncCallback(ReceiveCallback), state);
                    response = state.sb.ToString();
				} else {
					// All the data has arrived; put it in response.
					if (state.sb.Length > 1) {
						response = state.sb.ToString();
					}
					// Signal that all bytes have been received.
					receiveDone.Set();
				}
			} catch (Exception e) {
				print(e.ToString());
			print ("FAILED TO RECEIVE CALLBACK");
			Application.Quit();
			}
		}

		private static void Send(Socket client, String data) {
            try {
                // Convert the string data to byte data using ASCII encoding.
                byte[] byteData = Encoding.Unicode.GetBytes(data);

                // Begin sending the data to the remote device.
                client.BeginSend(byteData, 0, byteData.Length, 0,
                                 new AsyncCallback(SendCallback), client);
            }
            catch (Exception e) {
                print("FAILED TO SEND");
                Application.Quit();
            }
		}
		
		private static void SendCallback(IAsyncResult ar) {
			try {
				// Retrieve the socket from the state object.
				Socket clientTemp = (Socket) ar.AsyncState;

				// Complete sending the data to the remote device.
				int bytesSent = clientTemp.EndSend(ar);

				print("Sent {0} bytes to server." + bytesSent);
				// Signal that all bytes have been sent.
				sendDone.Set();

			} catch (Exception e) {
				print(e.ToString());
				print ("FAILED TO SEND CALLBACK");
				Application.Quit();
			}
		}

	// State object for receiving data from remote device.
	public class StateObject {
		// Client socket.
		public Socket workSocket = null;
		// Size of receive buffer.
		public const int BufferSize = 256;
		// Receive buffer.
		public byte[] buffer = new byte[BufferSize];
		// Received data string.
		public StringBuilder sb = new StringBuilder();
	}

	

}