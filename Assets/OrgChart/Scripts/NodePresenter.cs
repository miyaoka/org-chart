using UnityEngine;
using System.Collections;
using UniRx;

public class NodePresenter : MonoBehaviour {

  //view
  [SerializeField] public RectTransform childNodes;

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

}
