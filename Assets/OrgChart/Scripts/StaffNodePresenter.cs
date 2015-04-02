using UnityEngine;
using System.Collections;
using UniRx;
using UnityEngine.EventSystems;

public class StaffNodePresenter : NodePresenter {

  //view
  [SerializeField] GameObject contentUI;
  [SerializeField] GameObject staffUI;
  [SerializeField] GameObject emptyUI;
  [SerializeField] UILineRenderer familyLine;
  private float familyLineHeight = 19.0F;

  //model
  public ReactiveProperty<int?> staffId =  new ReactiveProperty<int?>();  
  public IntReactiveProperty tier = new IntReactiveProperty();
  public bool moved;
  IObservable<Vector2> parentDelta;
  IObservable<Vector2> thisDelta;

  CompositeDisposable eventResources = new CompositeDisposable();

	void Start () {
    StaffDataPresenter staff = gameObject.GetComponentInChildren<StaffDataPresenter> ();
    CanvasGroup cg = contentUI.GetComponent<CanvasGroup> ();

    staffId
      .Subscribe(x => {
        if(x.HasValue){
          staff.staffData = GameController.Instance.staffDataList[x.Value];
        }
        isAssigned.Value = x.HasValue;
      })
      .AddTo(eventResources);

    isAssigned
      .Subscribe (_ => {
        staffUI.SetActive(_);
        emptyUI.SetActive(!_);
      })
      .AddTo(eventResources);

    //hide content view if no assign & no children
    isAssigned
      .CombineLatest (childCountStream, (a, c) => (a || 0 < c ))
      .Subscribe (hasContent => {
        cg.alpha = hasContent ? 1 : 0;
      })
      .AddTo(eventResources);

    thisDelta = 
      Observable
        .EveryUpdate()
        .Select(_ => (this.transform as RectTransform).sizeDelta)
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
