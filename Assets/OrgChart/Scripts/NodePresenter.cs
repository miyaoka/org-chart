using UnityEngine;
using System.Collections;
using UniRx;

public class NodePresenter : MonoBehaviour {

  //view
  [SerializeField] public RectTransform childNodes;

  //model
  public IObservable<int> childCountStream;
  public ReadOnlyReactiveProperty<bool> hasChild { get; private set; }

  void Awake(){
    //define model

    childCountStream = 
      Observable
        .EveryUpdate ()
        .Select (_ => childNodes.childCount)
        .DistinctUntilChanged ();

    hasChild = 
      childCountStream
        .Select (c => 0 < c)
        .ToReadOnlyReactiveProperty ();
  }
  void Start(){
  }

}
