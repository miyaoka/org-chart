using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;

public class EndTurnButton : MonoBehaviour {

  [SerializeField] Text btnText;
  void Start(){
    GameController.Instance.onQuest
      .Subscribe (q => {
        btnText.text = q ? "Next Year" : "Go Quest";
    });
  }
	public void onClick(){
    GameController.Instance.nextPhase ();
//		EventManager.Instance.TriggerEvent(new EndTurnEvent() );	
	}
}
