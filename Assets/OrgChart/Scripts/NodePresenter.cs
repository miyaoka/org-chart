using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class NodePresenter : MonoBehaviour {

  //view
  [SerializeField] public RectTransform childNodes;




  //model
  public ReactiveProperty<int> childCount { get; private set; }
  public ReactiveProperty<int> childCountTotal = new ReactiveProperty<int>();
  public ReactiveProperty<int> currentLevel = new ReactiveProperty<int>();
  public ReactiveProperty<int> currentLevelTotal = new ReactiveProperty<int>();
  public ReactiveProperty<int> manCount = new ReactiveProperty<int>();
  public ReactiveProperty<int> manCountTotal = new ReactiveProperty<int>();

  public ReadOnlyReactiveProperty<bool> hasChild { get; private set; }
  public ReactiveProperty<bool> isSection = new ReactiveProperty<bool>();



  public ReactiveProperty<int> tier = new ReactiveProperty<int> (0);
  public ReactiveProperty<bool> isHired = new ReactiveProperty<bool> ();
  public ReactiveProperty<bool> isRoot = new ReactiveProperty<bool> ();

  public ReactiveProperty<bool> isDragging = new ReactiveProperty<bool> ();
  public ReactiveProperty<bool> isAssigned = new ReactiveProperty<bool> (true);
  public ReadOnlyReactiveProperty<bool> isEmpty { get; private set; }

  public ReactiveProperty<string> name = new ReactiveProperty<string> ();
  public ReactiveProperty<int> gender = new ReactiveProperty<int> ();
  public ReactiveProperty<int> baseLevel =  new ReactiveProperty<int> ();  
  public ReactiveProperty<int> lastLevel =  new ReactiveProperty<int> ();  
  public ReactiveProperty<int> age = new ReactiveProperty<int> ();
  public ReactiveProperty<float> health =  new ReactiveProperty<float> ();  

  public ReactiveProperty<StaffNodePresenter> parentNode = new ReactiveProperty<StaffNodePresenter>();
  public ReactiveProperty<int?> parentDiff = new ReactiveProperty<int?>();

  CompositeDisposable childResources = new CompositeDisposable();


  void Awake(){
    //define model

    var childStream = childNodes.gameObject.OnTransformChildrenChangedAsObservable ();

    childCount = 
      childStream
        .Select (_ => childNodes.childCount)
        .ToReactiveProperty ();

    isEmpty =
      isAssigned
        .CombineLatest (isDragging, (l, r) => !l || r)
        .ToReadOnlyReactiveProperty ();

    currentLevel = baseLevel
      .CombineLatest (childCount, (l, r) =>  l - r)
      .CombineLatest(isEmpty, (l, r) => r ? 0 : l)
      .ToReactiveProperty ();

    childCount
      .Subscribe (_ => watchChildSum ())
      .AddTo (this);
       
    hasChild = 
      childCount
        .Select (c => 0 < c)
        .ToReadOnlyReactiveProperty ();

    manCount =
      isEmpty
        .Select (a => a ? 0 : 1)
        .ToReactiveProperty ();

    watchChildSum ();

  }
  void watchChildSum(){
    childResources.Clear ();

    var lvList = new List<ReactiveProperty<int>> {currentLevel};
    var ccList = new List<ReactiveProperty<int>> {childCount};
    var mcList = new List<ReactiveProperty<int>> {manCount};
    foreach (Transform child in childNodes) {
      var node = child.GetComponent<NodePresenter> ();
      lvList.Add (node.currentLevelTotal);
      ccList.Add (node.childCountTotal);
      mcList.Add (node.manCountTotal);
    }

    Observable
      .CombineLatest (lvList.ToArray ())
      .Select (list => list.Sum())
      .Subscribe (v => currentLevelTotal.Value = v)
      .AddTo (childResources);

    Observable
      .CombineLatest (ccList.ToArray ())
      .Select (list => list.Sum())
      .Subscribe (v => childCountTotal.Value = v)
      .AddTo (childResources);

    Observable
      .CombineLatest (mcList.ToArray ())
      .Select (list => list.Sum ())
      .Subscribe (v => manCountTotal.Value = v)
      .AddTo (childResources);    
  }



}
