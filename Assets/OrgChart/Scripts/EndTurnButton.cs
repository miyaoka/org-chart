using UnityEngine;
using System.Collections;

public class EndTurnButton : MonoBehaviour {

	public void onClick(){
    GameController.Instance.nextPhase ();
//		EventManager.Instance.TriggerEvent(new EndTurnEvent() );	
	}
}
