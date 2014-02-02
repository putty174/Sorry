//AFSHIN MAHINI 2013 - 2014

using UnityEngine;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System;
using System.Diagnostics;
using System.Threading;

public class Sockets : MonoBehaviour {
	
	
	const string SERVER_LOCATION = "128.195.11.129";
	const int SERVER_PORT = 4414; //FILL THESE OUT FOR YOUR OWN SERVER
	
	public TcpClient client; 

	public NetworkStream nws;
	public int clientNumber;
	public bool startGame;
	public bool connected;
	
	public DateTime dt;
	
	public Thread t = null; 
		
	protected static bool threadState = false;
	
	public Queue recvBuffer;
	
	
	public Sockets()
	{
		
		connected = false;
		recvBuffer = new Queue();
		
		//uniClock = new Stopwatch();
		//dt = NTPTime.getNTPTime ( dt, ref uniClock );
		
	}
	
	public bool Connect ()
	{
		//********* COMPLETE THE FOLLOWING CODE
		//********* ESTABLISH CONNECTION THEN MAKE THREAD TO READ BYTES FROM STREAM
		try
		{
			client = new TcpClient();
			client.Connect(SERVER_LOCATION, SERVER_PORT);
			nws = client.GetStream();
			ThreadSock ts = new ThreadSock(nws, this);
			t = new Thread(new ThreadStart(ts.Service));
			t.IsBackground = true;
			t.Start();
		}
		catch ( Exception ex )
		{
			print ( ex.Message + " : OnConnect port " + SERVER_PORT);
		}
		
		if ( client == null ) return false;
		else
		return connected = client.Connected;
	}
	
	public bool Disconnect ()
	{
		//********* COMPLETE THE FOLLOWING CODE
		try
		{
			t.Abort();
			nws.Close();
			clientNumber--;
			if(clientNumber == 0)
				client.Close();
		}
		catch ( Exception ex )
		{
			print ( ex.Message + " : OnDisconnect" );
			return false;
		}
		return true;
	}
	
	public void SendTCPPacket ( byte toSend )
	{
		//********* COMPLETE THE FOLLOWING CODE
		try
		{	
			byte[] writeBuffer = new byte[1];
			writeBuffer[0] = toSend;
			nws.WriteByte(toSend);
		}
		catch ( Exception ex )
		{
			print ( ex.Message + ": OnTCPPacket" + toSend);
		}	
	}
	
	public void measureLatency () //UN-NECESSARY
	{
	}
	
	public int returnLatency(){//UN-NECESSARY
		//return latency;
		return 0;
	}
	
	
	public void endThread(){
		threadState = false;
	}
	
	public void testThread()
	{
		
		try
		{
			if ( t!= null && !threadState  )
			{
				print ( "thread aborted");
				t.Abort();
				threadState = !threadState;	
			}
		}
		catch ( Exception ex )
		{
			print ( ex.Message + " : testThread ");
		}
	}
	
}
