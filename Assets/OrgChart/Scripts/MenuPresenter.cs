using UnityEngine;
using System.Collections;
using UniRx;

public class MenuPresenter : MonoBehaviour {
  [SerializeField] GameObject dismissArea;
  [SerializeField] GameObject endTurnBtn;
  [SerializeField] GameObject resetBtn;
  [SerializeField] GameObject info;
  [SerializeField] CanvasGroup recruits;


	void Start () {
    GameController.Instance.draggingNode
      .Subscribe (n => {
        bool d = n != null;
        dismissArea.SetActive(d && n.isHired.Value);
//        endTurnBtn.SetActive(!d);
//        info.SetActive(!d);
        recruits.alpha = d ? 0 : 1;
        recruits.blocksRaycasts = !d;
    });

	}
	

}
