using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class NodePresenter : MonoBehaviour {

  //view
  [SerializeField] public RectTransform childNodes;

  //model
//  public IObservable<int> childCountStream;
  public IObservable<Unit> childStream;
  public ReadOnlyReactiveProperty<bool> hasChild { get; private set; }
  public ReadOnlyReactiveProperty<int> childCount { get; private set; }
  public ReactiveProperty<int> childCountTotal = new ReactiveProperty<int>();
  public ReactiveProperty<bool> isSection = new ReactiveProperty<bool>();

  CompositeDisposable childResources = new CompositeDisposable();

  void Awake(){
    //define model

    childStream = childNodes.gameObject.OnTransformChildrenChangedAsObservable ();

    childCount = 
      childStream
        .Select (_ => childNodes.childCount)
        .ToReadOnlyReactiveProperty ();

    childCount
      .Subscribe (x => {
        foreach(Transform child in childNodes){
          childResources.Dispose();
          childResources = new CompositeDisposable();
          child.GetComponent<NodePresenter>().childCountTotal
            .Subscribe(c => {
              getTotal();
            })
            .AddTo(child);
        }
        getTotal();
      });


    hasChild = 
      childCount
        .Select (c => 0 < c)
        .ToReadOnlyReactiveProperty ();


  }
  void getTotal(){
    int total = childNodes.childCount;
    foreach(Transform child in childNodes){
      total += child.GetComponent<NodePresenter> ().childCountTotal.Value;
    }
    childCountTotal.Value = total;
  }
  protected void Start(){

  }

}
