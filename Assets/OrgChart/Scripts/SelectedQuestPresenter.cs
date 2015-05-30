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
  public RectTransform healthContainerUI;

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
        healthContainerUI.OnRectTransformDimensionsChangeAsObservable ()
          .CombineLatest (q.health, (l, r) => r)
          .CombineLatest (q.maxHealth, (l, r) => Mathf.Max(0, r == 0 ? 0 : l / r * healthContainerUI.sizeDelta.x) )
          .Subscribe (w => healthUI.sizeDelta = new Vector2 (w, healthUI.sizeDelta.y))
          .AddTo (q);


        q.title
          .SubscribeToText (titleText)
          .AddTo (q);


        q.health
          .Select (v => v.ToString ("N0"))
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
