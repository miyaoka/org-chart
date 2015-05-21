using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ProjectPresenter : MonoBehaviour, IPointerDownHandler {
  #region IPointerDownHandler implementation
  public void OnPointerDown (PointerEventData eventData)
  {
    isSelected.Value = !isSelected.Value;
  }
  #endregion

  [SerializeField] Text titleText;
  [SerializeField] Text manPowerText;
  [SerializeField] Text rewardText;
  [SerializeField] Text chanceText;
  [SerializeField] Text durationText;
  [SerializeField] Text doneText;
  [SerializeField] RectTransform durationUI;
  [SerializeField] RectTransform doneUI;

  public ReactiveProperty<bool> isSelected = new ReactiveProperty<bool>();
  public ReactiveProperty<string> title = new ReactiveProperty<string>();
  public ReactiveProperty<int> manPower = new ReactiveProperty<int>();
  public ReactiveProperty<int> reward = new ReactiveProperty<int>();
  public ReactiveProperty<float> chance = new ReactiveProperty<float>();
  public ReactiveProperty<int> duration = new ReactiveProperty<int>();
  public ReactiveProperty<int> done = new ReactiveProperty<int>();

  CompositeDisposable eventResources = new CompositeDisposable();

	// Use this for initialization
	void Start () {

    title
      .SubscribeToText (titleText)
      .AddTo (eventResources);

    manPower
      .Select (v => "mp: " + v.ToString ("N0"))
      .SubscribeToText (manPowerText)
      .AddTo (eventResources);

    reward
      .Select (v => "" + v.ToString ("N0"))
      .SubscribeToText (rewardText)
      .AddTo (eventResources);

    chance
      .Select (v => "" + v.ToString("P0") )
      .SubscribeToText (chanceText)
      .AddTo (eventResources);

    duration
      .SubscribeToText (durationText)
      .AddTo (eventResources);

    done
      .SubscribeToText (doneText)
      .AddTo (eventResources);

    duration
      .Subscribe (v => durationUI.sizeDelta = new Vector2( (float)duration.Value * 10, 10) )
      .AddTo (eventResources);

    done
      .Subscribe (v => doneUI.sizeDelta = new Vector2( (float)done.Value * 10, 10) )
      .AddTo (eventResources);

	
	}
	
  void OnDestroy()
  {
    eventResources.Dispose();
  }

}
