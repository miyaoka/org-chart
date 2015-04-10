using UnityEngine;
using System.Collections;
using UniRx;

public class MenuPresenter : MonoBehaviour {
  [SerializeField] GameObject dismissArea;
  [SerializeField] GameObject endTurnBtn;
  [SerializeField] GameObject resetBtn;
  [SerializeField] GameObject info;


	void Start () {
    GameController.Instance.draggingNode
      .Where (n => n != null)
      .Subscribe (n => {
        Debug.Log(n.isHired);
    });

	}
	

}
