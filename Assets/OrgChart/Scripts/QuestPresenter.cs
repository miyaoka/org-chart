﻿using UnityEngine;
using System.Collections;
using UniRx;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class QuestPresenter : MonoBehaviour, IPointerClickHandler {
  #region IPointerClickHandler implementation

  public void OnPointerClick (PointerEventData eventData)
  {
    GameController.Instance.selectedQuest.Value = this;
  }

  #endregion

  public Text healthText;
  public Text attackText;
  public Text rewardText;


  public ReactiveProperty<bool> isSelected = new ReactiveProperty<bool>();
  public ReactiveProperty<string> title = new ReactiveProperty<string>();
  public ReactiveProperty<float> maxHealth = new ReactiveProperty<float>();
  public ReactiveProperty<float> health = new ReactiveProperty<float>();
  public ReactiveProperty<float> attack = new ReactiveProperty<float>();
  public ReactiveProperty<float> reward = new ReactiveProperty<float>();
	// Use this for initialization
	protected void Start () {

    title.Value = "test quest";
    maxHealth.Value = 100.5f;
    health.Value = 80.2f;



    health
      .Select (v => v.ToString ("N0"))
      .SubscribeToText (healthText)
      .AddTo (this);

    attack
      .Select (v => v.ToString ("N0"))
      .SubscribeToText (attackText)
      .AddTo (this);

    reward
      .Select (v => v.ToString ("N0"))
      .SubscribeToText (rewardText)
      .AddTo (this);
    
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
