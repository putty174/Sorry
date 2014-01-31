//AFSHIN MAHINI 2013 - 2014

using UnityEngine;
using System.Collections;

public class Blue1 : MonoBehaviour {

	private Vector3 lastPosition ;
	private GameProcess gp;
	
	
	void Start () 
	{
		gp = GameObject.Find("GameProcess").GetComponent<GameProcess>();
		lastPosition = gameObject.transform.position ;
	}
	
	void Update () 
	{
	}
	
	void OnMouseDrag()
	{
		if ( gp.turn == 3  )
		{
				if ( gp.takingTurn == true )
				{
					Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					point.z = 500;
			        gameObject.transform.position = point;
				}
				else gp.printGui ( "Please roll first.\n");        
		}
		      
	}
	
	void OnMouseUp()
	{
		
		if (  gp.turn == 3 && gp.takingTurn == true ) 	
		{
			if ( !PawnHandle.findMove ( this.transform.position, ref  gp   ) ) 
			{
				this.transform.position = lastPosition ;
			}
		
			else
			{//********* COMPLETE THE FOLLOWING CODE
				if ( gp.winningMove != 0 )
				{
					gp.sendEndGame();
				}
				else
				{
					
					
					if ( gp.playerPositions[2] > 60 ) gp.playerPositions[2] -= 60;
					
					PawnHandle.doesCollide ( ref gp ) ;
					
					lastPosition = this.transform.position ;
					
					gp.sendPositions();
					
					gp.nextTurn();
				}
			}
		}
	}
}
