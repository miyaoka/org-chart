using UnityEngine;
using System.Collections;
using UniRx;
using UnityEngine.EventSystems;

public class StaffNodePresenter : NodePresenter {

  //view
  [SerializeField] GameObject contentUI;
  [SerializeField] GameObject staffUI;
  [SerializeField] GameObject emptyUI;
  [SerializeField] GameObject familyLineUI;
  private UILineRenderer familyLine;
  private float familyLineHeight = 19.0F;

  //model
  public ReactiveProperty<int?> staffId =  new ReactiveProperty<int?>();  
  public IntReactiveProperty tier = new IntReactiveProperty();
  public bool isMoved;
  public bool isHired;
  public ReactiveProperty<bool> isDragging = new ReactiveProperty<bool> (false);
  IObservable<Vector2> parentDelta;
  IObservable<Vector2> thisDelta;

  CompositeDisposable eventResources = new CompositeDisposable();

	void Start () {
    StaffDataPresenter staff = gameObject.GetComponentInChildren<StaffDataPresenter> ();
    CanvasGroup cg = contentUI.GetComponent<CanvasGroup> ();
    familyLine = familyLineUI.GetComponent<UILineRenderer> ();



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
      .Subscribe (_ => {
        Destroy (gameObject);
      })
      .AddTo (eventResources);

    //hide if no content
    hasContent
      .Subscribe (c => {
        cg.alpha = c ? 1 : 0;
        showFamilyLine(isHired && c);
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
	}
  void OnDestroy()
  {
    eventResources.Dispose();
  }
  void showFamilyLine(bool b){
    familyLineUI.SetActive (b);
  }

  void drawFamilyLine(Vector2 lineDelta){
    familyLine.Points = new Vector2[] { 
      new Vector2(lineDelta.x, 0), 
      new Vector2(lineDelta.x, 8),//familyLineHeight * .4F),
      new Vector2(lineDelta.y, 8),//familyLineHeight * .4F),
      //      new Vector2(pt.y, familyLineHeight * .6F),      
      new Vector2(lineDelta.y, familyLineHeight)
    };
    familyLine.SetVerticesDirty();    
  }

}
