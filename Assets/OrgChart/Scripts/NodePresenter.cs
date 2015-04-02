using UnityEngine;
using System.Collections;
using UniRx;

public class NodePresenter : MonoBehaviour {

  //view
  [SerializeField] public RectTransform childNodes;

  //model
  public ReactiveProperty<bool> isAssigned = new ReactiveProperty<bool> (true);
  public IObservable<int> childCountStream;
  CompositeDisposable eventResources = new CompositeDisposable();

  void Awake(){
    //define model
    childCountStream = 
      Observable
        .EveryUpdate ()
        .Select (_ => childNodes.childCount)
        .DistinctUntilChanged ();
  }
  public bool hasChild(){
    return 0 < childNodes.childCount;
  }

}
