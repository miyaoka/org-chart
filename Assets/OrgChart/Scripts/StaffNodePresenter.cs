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
  public ReactiveProperty<bool> isAssigned = new ReactiveProperty<bool> ();
  IObservable<Vector2> parentDelta;
  IObservable<Vector2> thisDelta;

  CompositeDisposable eventResources = new CompositeDisposable();

	void Start () {
    StaffDataPresenter staff = gameObject.GetComponentInChildren<StaffDataPresenter> ();
    CanvasGroup cg = contentUI.GetComponent<CanvasGroup> ();

    staffId
      .Subscribe(x => {
        if(x.HasValue){
          staff.staffData = GameController.staffDataList[x.Value];
        }
        isAssigned.Value = x.HasValue;
      })
      .AddTo(eventResources);

    isAssigned
      .Subscribe (x => {
        staffUI.SetActive(x);
        emptyUI.SetActive(!x);

        //hide view if no assign no children
        cg.alpha = (!x && 0 == childNodes.childCount ) ? 0 : 1;
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
  public void onDrop(PointerEventData eventData){
    Debug.Log ("on drop");

    GameObject draggedItem = eventData.pointerDrag;
    if(draggedItem.transform.parent == childNodes){
      return;
    }



    StaffPresenter sp = draggedItem.GetComponent<StaffPresenter>();
    sp.dragPointer.transform.SetParent(childNodes, false);
    sp.dragPointer.GetComponent<StaffPresenter>().setPointer(false);
    if(sp.childrenContainer.childCount == 0){
      Destroy(draggedItem);
    }
    //    draggedItem.transform.SetParent(chindrenContainer, true);

    EventManager.Instance.TriggerEvent (new ChartChangeEvent() );


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
