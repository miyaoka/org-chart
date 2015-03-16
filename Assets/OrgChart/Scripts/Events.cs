using UnityEngine;
using System.Collections;


public class ChartChangeEvent : GameEvent {	
	public ChartChangeEvent(){
	}
}

public class StaffBeginDragEvent : GameEvent {	
	public GameObject dragged { get; private set; }
	public StaffBeginDragEvent(GameObject dragged){
		this.dragged = dragged;
	}
}
public class StaffEndDragEvent : GameEvent {	
	public StaffEndDragEvent(){
	}
}