//AFSHIN MAHINI 2013 - 2014

using UnityEngine;
using System;
using System.Collections;
using System.Net.Sockets;
using System.Net;

public class ThreadSock : MonoBehaviour 
{
	private NetworkStream nws;
	private byte[] streamBuffer;
	private byte byteBuffer;
	private byte tempBuffer;
	private Sockets socks;
	
	//********* COMPLETE THE FOLLOWING CODE
	public ThreadSock (NetworkStream nwsIn, Sockets inSocket )
	{
		nws = nwsIn;
		socks = inSocket;
		streamBuffer = new byte[1];
	}
	//********* COMPLETE THE FOLLOWING CODE
	//********* READ THE STREAM, ADD TO QUEUE, BE THREAD SAFE
	public void Service ()
	{	
		try
		{
			while(socks.connected)
			{
				nws.Read(streamBuffer,0,1);
				lock (socks.recvBuffer)
				{
					socks.recvBuffer.Enqueue(streamBuffer[0]);
				}
			}
		}
		catch ( Exception ex )
		{
			print ( ex.Message + " : Thread loop" );
			
		}
		
	}
	
}
