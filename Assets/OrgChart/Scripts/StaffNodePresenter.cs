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
  [SerializeField] Text childCountUI;
  private UILineRenderer familyLine;
  private const float familyLineHeight = 19.0F;

  //model
  public int nodeId;
  public ReactiveProperty<bool> isAssigned = new ReactiveProperty<bool> (true);
  public ReactiveProperty<int> tier = new ReactiveProperty<int> (0);
  public ReactiveProperty<bool> isHired = new ReactiveProperty<bool> (false);
  public bool isMoved;
  public ReactiveProperty<bool> isDragging = new ReactiveProperty<bool> (false);

  public ReactiveProperty<int> currentSkill = new ReactiveProperty<int>();

  public ReactiveProperty<int> baseSkill =  new ReactiveProperty<int> ();  
  public ReactiveProperty<int> lastSkill =  new ReactiveProperty<int> ();  
  public ReactiveProperty<int> age = new ReactiveProperty<int> ();
  public ReactiveProperty<Color> shirtsColor = new ReactiveProperty<Color>();
  public ReactiveProperty<Color> tieColor = new ReactiveProperty<Color>();
  public ReactiveProperty<Color> suitsColor = new ReactiveProperty<Color>();
  public ReactiveProperty<Color> faceColor = new ReactiveProperty<Color>();
  public ReactiveProperty<Color> hairColor = new ReactiveProperty<Color>();
  public ReactiveProperty<int> job = new ReactiveProperty<int>();

  public ReactiveProperty<StaffNodePresenter> parentNode = new ReactiveProperty<StaffNodePresenter>();
  public ReactiveProperty<int?> parentDiff = new ReactiveProperty<int?>();

  IObservable<Vector2> parentDelta;
  IObservable<Vector2> thisDelta;

  CompositeDisposable eventResources = new CompositeDisposable();


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
    base.Start ();
    CanvasGroup cg = contentUI.GetComponent<CanvasGroup> ();
    familyLine = familyLineUI.GetComponent<UILineRenderer> ();
    Image bg = GetComponent<Image> ();

    currentSkill = baseSkill
      .CombineLatest (childCount, (s, c) =>  s - c )
      .ToReactiveProperty ();

    parentNode
      .Where (pn => pn != null)
      .Subscribe (pn => {
        parentDiff = pn.currentSkill
          .CombineLatest(currentSkill, (l, r) => (int?)l - r)
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
      .SubscribeToText (childCountUI)
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
      new Vector2(lineDelta.x, 10),//familyLineHeight * .4F),
      new Vector2(lineDelta.y, 10),//familyLineHeight * .4F),
      new Vector2(lineDelta.y, familyLineHeight)
    };
    familyLine.SetVerticesDirty();    
  }
  void OnDestroy()
  {
    eventResources.Dispose();
  }

}
