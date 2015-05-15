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

  public ReactiveProperty<bool> isSelected = new ReactiveProperty<bool>();
  public ReactiveProperty<string> title = new ReactiveProperty<string>();
  public ReactiveProperty<int> manPower = new ReactiveProperty<int>();
  public ReactiveProperty<int> reward = new ReactiveProperty<int>();
  public ReactiveProperty<float> chance = new ReactiveProperty<float>();

  CompositeDisposable eventResources = new CompositeDisposable();

	// Use this for initialization
	void Start () {

    title
      .SubscribeToText (titleText)
      .AddTo (eventResources);

    manPower
      .Select (v => "require: " + v.ToString ("N0"))
      .SubscribeToText (manPowerText)
      .AddTo (eventResources);

    reward
      .Select (v => "+" + v.ToString ("N0"))
      .SubscribeToText (rewardText)
      .AddTo (eventResources);

    chance
      .Select (v => "" + v.ToString("P0") )
      .SubscribeToText (chanceText)
      .AddTo (eventResources);

	
	}
	
  void OnDestroy()
  {
    eventResources.Dispose();
  }

}
