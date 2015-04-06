using UnityEngine;
using System.Collections;
using UniRx;

public class NodePresenter : MonoBehaviour {

  //view
  [SerializeField] public RectTransform childNodes;

  //model
  public ReactiveProperty<bool> isAssigned = new ReactiveProperty<bool> (true);
  public IObservable<int> childCountStream;
  public ReadOnlyReactiveProperty<bool> hasChild { get; private set; }
  CompositeDisposable eventResources = new CompositeDisposable();

  void Awake(){
    //define model

    childCountStream = 
      Observable
        .EveryUpdate ()
        .Select (_ => childNodes.childCount)
        .DistinctUntilChanged ();

    hasChild = 
      childCountStream
        .Select(c => 0 < c)
        .ToReadOnlyReactiveProperty ();
  }
  void Start(){
  }

}
