using UnityEngine;
using System.Collections;
using UniRx;
using UnityEngine.EventSystems;
using System.Reflection;
using UnityEngine.UI;
using UniRx.Triggers;

public class StaffNodePresenter : NodePresenter {

  //view
  [SerializeField] GameObject contentUI;
  [SerializeField] GameObject staffUI;
  [SerializeField] GameObject emptyUI;
  [SerializeField] GameObject familyLineUI;
  [SerializeField] Text childCountText;
  [SerializeField] Text levelCountText;
  private UILineRenderer familyLine;
  private const float familyLineHeight = 19.0F;

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
    CanvasGroup cg = contentUI.GetComponent<CanvasGroup> ();
    familyLine = familyLineUI.GetComponent<UILineRenderer> ();
    Image bg = GetComponent<Image> ();



    parentNode
      .Where (pn => pn != null)
      .Subscribe (pn => {
        parentDiff = pn.currentLevel
          .CombineLatest(currentLevel, (l, r) => (int?)l - r)
          .CombineLatest(pn.isAssigned, (l, r) => r ? l : null)
          .ToReactiveProperty ();
      })
      .AddTo(eventResources);
    /*
    parentDiff = parentNode
      .Where(pn => pn != null)
      .Select(pn => pn.currentSkill.Value)
      .CombineLatest(currentSkill, (l, r) => (int?)l - r)
      .ToReactiveProperty ();
*/

    isAssigned
      .Subscribe (_ => {
        staffUI.SetActive(_);
        emptyUI.SetActive(!_);
      })
      .AddTo(eventResources);

    //observe content (assign or children)
    IObservable<bool> hasContent =
      isAssigned
        .CombineLatest (hasChild, (assign, child) => (assign || child));

    //destory if no content & no dragging
    hasContent
      .CombineLatest (isDragging, (c, d) => c || d)
      .Where(exist => exist == false)
      .Subscribe (_ => GameController.Instance.destroyNode(gameObject))
      .AddTo (eventResources);

    //hide if no content
    hasContent
      .Subscribe (c => {
        cg.alpha = c ? 1 : 0;
        showFamilyLine(isHired.Value && c);
      })
      .AddTo(eventResources);

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
      .AddTo(eventResources);

    childCount
      .CombineLatest(childCountTotal, (l,r) => l + "/" + r)
      .SubscribeToText (childCountText)
      .AddTo (eventResources);

    currentLevelTotal
      .SubscribeToText (levelCountText)
      .AddTo (eventResources);

    childCountTotal
      .Subscribe (c => {
        if(3 < c && tier.Value == 1){
          isSection.Value = true;
        }else {
          isSection.Value = false;
        }
      })
      .AddTo (eventResources);

    isSection
      .Subscribe (s => {
        bg.enabled = s;
      })
      .AddTo (eventResources);
        
	}
  void showFamilyLine(bool show){
    familyLineUI.SetActive (show);
  }

  void drawFamilyLine(Vector2 lineDelta){
    familyLine.Points = new Vector2[] { 
      new Vector2(lineDelta.x, 0), 
      new Vector2(lineDelta.x, 4),//familyLineHeight * .4F),
      new Vector2(lineDelta.y, 4),//familyLineHeight * .4F),
      new Vector2(lineDelta.y, familyLineHeight)
    };
    familyLine.SetVerticesDirty();    
  }
  void OnDestroy()
  {
    eventResources.Dispose();
  }

}
