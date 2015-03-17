using UnityEngine;
using System.Collections;

public class EndTurnButton : MonoBehaviour {

	public void onClick(){
		EventManager.Instance.TriggerEvent(new EndTurnEvent() );	
	}
}
