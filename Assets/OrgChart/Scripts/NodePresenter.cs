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

  public ReadOnlyReactiveProperty<bool> hasChild { get; private set; }
  public ReactiveProperty<bool> isSection = new ReactiveProperty<bool>();



  public ReactiveProperty<int> tier = new ReactiveProperty<int> (0);
  public ReactiveProperty<bool> isAssigned = new ReactiveProperty<bool> (true);
  public ReactiveProperty<bool> isHired = new ReactiveProperty<bool> (false);
  public bool isMoved;
  public ReactiveProperty<bool> isDragging = new ReactiveProperty<bool> (false);

  public ReactiveProperty<string> name = new ReactiveProperty<string> ();
  public ReactiveProperty<int> gender = new ReactiveProperty<int> ();
  public ReactiveProperty<int> baseLevel =  new ReactiveProperty<int> ();  
  public ReactiveProperty<int> lastLevel =  new ReactiveProperty<int> ();  
  public ReactiveProperty<int> age = new ReactiveProperty<int> ();
  public ReactiveProperty<float> health =  new ReactiveProperty<float> ();  

  public ReactiveProperty<StaffNodePresenter> parentNode = new ReactiveProperty<StaffNodePresenter>();
  public ReactiveProperty<int?> parentDiff = new ReactiveProperty<int?>();

  CompositeDisposable childResources = new CompositeDisposable();

  public ReactiveProperty<StaffModel> staff = new ReactiveProperty<StaffModel> ();


  void Awake(){
    //define model

    var childStream = childNodes.gameObject.OnTransformChildrenChangedAsObservable ();

    childCount = 
      childStream
        .Select (_ => childNodes.childCount)
        .ToReactiveProperty ();

    currentLevel = baseLevel
      .CombineLatest (childCount, (l, r) =>  l - r)
      .CombineLatest(isAssigned, (l,r) => r ? l : 0)
      .ToReactiveProperty ();

    childCount
      .Subscribe (x => {
        childResources.Clear ();

        var lvList = new List<ReactiveProperty<int>> {currentLevel};
        var ccList = new List<ReactiveProperty<int>> {childCount};
        foreach (Transform child in childNodes) {
          var node = child.GetComponent<NodePresenter> ();
          lvList.Add (node.currentLevelTotal);
          ccList.Add (node.childCountTotal);
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

      })
      .AddTo (this);
       
    hasChild = 
      childCount
        .Select (c => 0 < c)
        .ToReadOnlyReactiveProperty ();

  }



}
