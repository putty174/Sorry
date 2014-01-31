//AFSHIN MAHINI 2013 - 2014

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GameProcess : MonoBehaviour {
	
	//PUBLIC MEMBERS
	public int clientNumber;
	public int turn ;
	public bool takingTurn;
	public int roll;
	public bool startGame;
	public int backMove ;
	public int forwardMove ;
	public int winningMove ;
	public readonly byte[] commands = {0x00, 0x40, 0x80, 0xC0};//connect//turn//roll//move
	
	public enum codes  {init, turn, roll, move }; // 00   01    10     11
	
	public Player1 p1;
	public Player2 p2;
	public Player3 p3;
	public Player4 p4;
	
	public GameObject[] pawnObjects; 
	
	public int[] playerPositions;
	
	//PRIVATE MEMBERS
	private Sockets socks;
	private byte byteBuffer;
	private byte tempBuffer;
	private Gui gui;
	private int moveCommands ;
	
	private readonly string[] moveOptions = { "You can move a pawn from the start or move forward 1 space.",
									          "You can move a pawn from the start or move forward 2 spaces.",
											  "Move forward 3 spaces.",
											  "Move backward 2 spaces.",
											  "Move forward 5 spaces.",
											  "Move forward 7 spaces.",
											  "Move forward 8 spaces.",
									          "Move forward 10 spaces or move backward 1 space.",
											  "Move forward 11 spaces or bump switch places with an opponent. If it is impossible to move forward 11 spaces, and there are no opponent pawns on the board, then you will have to forfeit your turn.",
											  "Move forward 12 spaces.",
											  "Sorry! : You can use this roll to bump an opponent's pawn to the start."};
	
	void Start () 
	{
		turn = -1;
		socks = new Sockets();
		takingTurn = false;
		moveCommands = 1;
		
		
		pawnObjects = new GameObject[4];
		playerPositions = new int[4]; 
		
		gui = GameObject.Find("Gui").GetComponent<Gui>();
		
		p1 = GameObject.Find("Player1").GetComponent<Player1>();
		p2 = GameObject.Find("Player2").GetComponent<Player2>();
		p3 = GameObject.Find("Player3").GetComponent<Player3>();
		p4 = GameObject.Find("Player4").GetComponent<Player4>();
		
		winningMove = 0;
		
		pawnObjects[0] = GameObject.Find("Green1").GetComponent<Green1>().gameObject;
		pawnObjects[1] = GameObject.Find("Red1").GetComponent<Red1>().gameObject;
		pawnObjects[2] = GameObject.Find("Blue1").GetComponent<Blue1>().gameObject;
		pawnObjects[3] = GameObject.Find("Yellow1").GetComponent<Yellow1>().gameObject;
	
	}
	
	void Update () 
	{
		//********* COMPLETE THE FOLLOWING CODE
		//********* THIS IS MAIN LOOP WHERE GAME INFORMATION IS PROCESSED FROM THE QUEUE
		//********* READ THE QUEUE AND PROCESS THE GAME, BE THREAD SAFE
		if ( socks.recvBuffer.Count > 0 ) //Items in queue
		{
			lock(socks.recvBuffer)
			{
				while(socks.recvBuffer.Count > 0)
				{
					byteBuffer = (byte) socks.recvBuffer.Dequeue();
					tempBuffer = (byte) (byteBuffer >> 6);
					byte iBuffer =  (byte) (byteBuffer & 0x3F);
					//Debug.Log ("Byte: " + byteBuffer + " Command: " + tempBuffer + " Instruction: " + iBuffer);
					//Process Queue
					switch ( tempBuffer )
					{
					case 0:
					{
						if(iBuffer > 0 && iBuffer < 5)
						{
							if(!startGame)
							{
								clientNumber = iBuffer;
							}
							else
							{
								winningMove = iBuffer;
								printGui("Player " + winningMove + " Wins!");
							}
						}
						else if (iBuffer == 5)
						{
							startGame = true;
							for(int i = 0 ; i < 4; i++)
							{
								playerPositions[i] = 0;
							}
						}
						else
						{
							Debug.Log("Invalid Start Command");
						}
						break;
					}
					case 1:
					{
						if(iBuffer > 0 && iBuffer < 5)
						{
							turn = iBuffer;
						}
						break;
					}
					case 2:
					{
						tempBuffer = (byte)(byteBuffer << 2);
						tempBuffer = (byte)(tempBuffer >> 2);
						gui.printGui(moveOptions[ tempBuffer - 1 ] );
						switch ( tempBuffer )
						{
						case 0x01 :
							backMove = forwardMove = 1;
							break;
						case 0x02 :
							backMove = forwardMove = 2;
							break;
						case 0x03 :
							backMove = forwardMove = 3;
							break;
						case 0x04 :
							backMove = forwardMove = -2;
							break;
						case 0x05 :
							backMove = forwardMove = 5;
							break;
						case 0x06 :
							backMove = forwardMove = 7;
							break;
						case 0x07 :
							backMove = forwardMove = 8;
							break;
						case 0x08 :
							backMove = -1;
							forwardMove = 10;
							break;
						case 0x09 :
							backMove = -100;
							forwardMove = 11;
							break;
						case 0x0A :
							backMove = forwardMove = 12;
							break;
						case 0x0B :
							backMove = forwardMove = -200;
							break;
						default :
							break;
						}
						
						roll = tempBuffer;
						if (turn == clientNumber) takingTurn = true;
						break;
					}
					case 3:
					{
						Debug.Log("Move Command: " + moveCommands);
						if(iBuffer > -1 && iBuffer < 61)
						{
							playerPositions[moveCommands-1] = iBuffer;
							
							Debug.Log("New Positions> Player 1 : " + playerPositions[0] + " Player 2: " + playerPositions[1] + " Player 3: "+ playerPositions[2] + " Player 4: " + playerPositions[3]);
							if(playerPositions[moveCommands-1] == 0)
							{
								pawnObjects[moveCommands-1].transform.position = PawnHandle.startPositions[moveCommands-1];
							}
							else
							{
								pawnObjects[moveCommands-1].transform.position = PawnHandle.positions[playerPositions[moveCommands-1]];
							}
							moveCommands++;
							if(moveCommands == 5)
							{
								moveCommands = 1;
							}
						}
						break;
					}
					}
				}
			}
		}
	}
	
	public Sockets returnSocket ()
	{
		return socks;
	}
	
	
	
	public void nextTurn (  )
	{
		//********* COMPLETE THE FOLLOWING CODE
		takingTurn = false;

		byteBuffer = 64;
		socks.SendTCPPacket(byteBuffer);
	}
	
	public void sendPositions ()
	{
		//********* COMPLETE THE FOLLOWING CODE
		for(int i = 0; i < 4; i++)
		{
			Debug.Log("Sending Positions> Player 1 : " + playerPositions[0] + " Player 2: " + playerPositions[1] + " Player 3: "+ playerPositions[2] + " Player 4: " + playerPositions[3]);
			byteBuffer = (byte) (192 + playerPositions[i]);
			socks.SendTCPPacket (byteBuffer);
		}
	}
	
	public void sendEndGame ()
	{
		//********* COMPLETE THE FOLLOWING CODE
		byteBuffer = (byte) clientNumber;
		socks.SendTCPPacket(byteBuffer);
	}
	
	public void printGui( string printStr )
	{
		this.gui.printGui(printStr );
	}
	
	
}
