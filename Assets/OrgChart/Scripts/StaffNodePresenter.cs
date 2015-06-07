﻿using UnityEngine;
using System.Collections;
using UniRx;
using UnityEngine.EventSystems;
using System.Reflection;
using UnityEngine.UI;
using UniRx.Triggers;

public class StaffNodePresenter : NodePresenter {

  //view
//  [SerializeField] GameObject contentUI;
  [SerializeField] GameObject staffUI;
  [SerializeField] GameObject emptyUI;
  [SerializeField] GameObject familyLineUI;
  [SerializeField] CanvasGroup panelCG;

  [SerializeField] Text childCountText;
  [SerializeField] Text levelCountText;
  private UILineRenderer familyLine;
  private const float familyLineHeight = 24.0F;

  //model


  IObservable<Vector2> parentDelta;
  IObservable<Vector2> thisDelta;



  public StaffData staffData{
    get {
      var sd = new StaffData();        
      foreach(FieldInfo fi in sd.GetType().GetFields()){
        object reactiveProp = this.GetType ().GetField (fi.Name).GetValue (this);
        sd.GetType().GetField(fi.Name).SetValue(
          sd, 
          reactiveProp.GetType ().GetProperty ("Value").GetValue(reactiveProp, null)
        );
      }
      return sd;
    }
    set {
      foreach(FieldInfo fi in value.GetType().GetFields()){
        object reactiveProp = this.GetType ().GetField (fi.Name).GetValue (this);
        reactiveProp.GetType ().GetProperty ("Value").SetValue(
          reactiveProp,
          value.GetType ().GetField (fi.Name).GetValue (value),
          null
        );
      }
    }
  }




	void Start () {
//    base.Start ();
    var gc = GameController.Instance;
    familyLine = familyLineUI.GetComponent<UILineRenderer> ();


    GameController.Instance.draggingNode
      .Select (d => d != null)
      .Subscribe (d => panelCG.blocksRaycasts = d ? false : true)
      .AddTo (this);


        /*
    health
      .Where (h => 0 >= h)
      .Subscribe (_ => isEmpty.Value = false)
      .AddTo (this);
*/

    parentNode
      .Where (pn => pn != null)
      .Subscribe (pn => {
        parentDiff = pn.currentLevel
          .CombineLatest(currentLevel, (l, r) => (int?)l - r)
          .CombineLatest(pn.isEmpty, (l, r) => r ? null : l)
          .ToReactiveProperty ();
      })
      .AddTo(this);



    isEmpty
      .Subscribe (e => {
        staffUI.SetActive(!e);
        emptyUI.SetActive(e);
      })
      .AddTo(this);


    //destory if no content on no dragging
    isDragging
      .CombineLatest (isAssigned, (l, r) => l || r)
      .CombineLatest (hasChild, (l, r) => l || r)
      .Where(exist => !exist)
      .Subscribe (_ => Destroy(gameObject))
      .AddTo (this);

    //hide if no content on dragging
    isDragging
      .CombineLatest(hasChild, (l, r) => l && !r)
      .Subscribe (c => {
        panelCG.alpha = c ? 0 : 1;
        showFamilyLine(isHired.Value && c);
      })
      .AddTo(this);

    thisDelta = 
      Observable
        .EveryUpdate()
        .Select(_ => (transform as RectTransform).sizeDelta)
        .DistinctUntilChanged();
    parentDelta = 
      Observable
        .EveryUpdate()
        .Select(_ => transform.parent ? (transform.parent.transform as RectTransform).sizeDelta : new Vector2())
        .DistinctUntilChanged();

    thisDelta
      .CombineLatest (parentDelta, (td, pd) => new Vector2 (
      td.x / 2, 
      pd.x / 2 - transform.position.x + (transform.parent ? transform.parent.transform.position.x : 0)
    ))
      .Subscribe (drawFamilyLine)
      .AddTo(this);

    childCount
      .CombineLatest(childCountTotal, (l,r) => l + "/" + r)
      .SubscribeToText (childCountText)
      .AddTo (this);

    currentLevelTotal
      .SubscribeToText (levelCountText)
      .AddTo (this);

    /*
    childCountTotal
      .Subscribe (c => {
        if(3 < c && tier.Value == 1){
          isSection.Value = true;
        }else {
          isSection.Value = false;
        }
      })
      .AddTo (this);

    isSection
      .Subscribe (s => {
        bg.enabled = s;
      })
      .AddTo (this);
    */
        
	}
  void showFamilyLine(bool show){
    familyLineUI.SetActive (show);
  }

  void drawFamilyLine(Vector2 lineDelta){
    familyLine.Points = new Vector2[] { 
      new Vector2(lineDelta.x, 0), 
      new Vector2(lineDelta.x, 10),//familyLineHeight * .4F),
      new Vector2(lineDelta.y, 10),//familyLineHeight * .4F),
      new Vector2(lineDelta.y, familyLineHeight)
    };
    familyLine.SetVerticesDirty();    
  }

}
