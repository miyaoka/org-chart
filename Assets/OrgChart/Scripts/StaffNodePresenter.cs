using UnityEngine;
using System.Collections;
using UniRx;

public class StaffNodePresenter : NodePresenter {

  //view
  [SerializeField] GameObject assignedNode;
  [SerializeField] GameObject emptyNode;
  [SerializeField] UILineRenderer familyLine;
  private float familyLineHeight = 19.0F;

  //model
  public ReactiveProperty<int?> staffId =  new ReactiveProperty<int?>();  
  public IntReactiveProperty tier = new IntReactiveProperty();
  IObservable<Vector2> parentDelta;
  IObservable<Vector2> thisDelta;

  CompositeDisposable eventResources = new CompositeDisposable();

	void Start () {
    StaffDataPresenter staff = gameObject.GetComponentInChildren<StaffDataPresenter> ();

    staffId
      .Subscribe(x => {
        if(x.HasValue){
          staff.staffData = GameController.staffDataList[x.Value];
        }
        assignedNode.SetActive(x.HasValue);
        emptyNode.SetActive(!x.HasValue);
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
