using UnityEngine;
using System.Collections;
using UniRx;
using UnityEngine.UI;
using UniRx.Triggers;


public class SelectedQuestPresenter : MonoBehaviour {
  public Text healthText;
  public Text attackText;
  public Text rewardText;
  public Text titleText;
  public RectTransform healthUI;

  public GameObject hasQuest;
  public GameObject noQuest;


	// Use this for initialization
	void Start () {

    GameController.Instance.selectedQuest
      .Select(q => q != null)
      .Subscribe (q => {
        hasQuest.SetActive(q);
        noQuest.SetActive(!q);
      });

    GameController.Instance.selectedQuest
      .Where(q => q)
      .Subscribe (q => {
        
        q.health
          .CombineLatest (q.maxHealth, (l, r) => Mathf.Max(0, r == 0 ? 0 : l / r ))
          .Subscribe (w => healthUI.anchorMax = new Vector2(w, 1))
          .AddTo (q);

        q.title
          .SubscribeToText (titleText)
          .AddTo (q);


        q.health
          .CombineLatest (q.maxHealth, (l, r) => l.ToString("N0") + "/" + r.ToString("N0") )
//          .Select (v => v.ToString ("N0"))
          .SubscribeToText (healthText)
          .AddTo (q);

        q.attack
          .Select (v => v.ToString ("N0"))
          .SubscribeToText (attackText)
          .AddTo (q);

        q.reward
          .Select (v => v.ToString ("N0"))
          .SubscribeToText (rewardText)
          .AddTo (q);        
    });



	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
