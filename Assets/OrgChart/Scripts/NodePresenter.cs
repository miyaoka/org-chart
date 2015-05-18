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

  public ReactiveProperty<int> currentLevel = new ReactiveProperty<int>();
  public ReactiveProperty<int> currentLevelTotal = new ReactiveProperty<int>();


  public ReactiveProperty<bool> isAssigned = new ReactiveProperty<bool> (true);
  public ReactiveProperty<int> tier = new ReactiveProperty<int> (0);
  public ReactiveProperty<bool> isHired = new ReactiveProperty<bool> (false);
  public bool isMoved;
  public ReactiveProperty<bool> isDragging = new ReactiveProperty<bool> (false);


  public ReactiveProperty<int> baseLevel =  new ReactiveProperty<int> ();  
  public ReactiveProperty<int> lastLevel =  new ReactiveProperty<int> ();  
  public ReactiveProperty<int> age = new ReactiveProperty<int> ();
  public ReactiveProperty<Color> shirtsColor = new ReactiveProperty<Color>();
  public ReactiveProperty<Color> tieColor = new ReactiveProperty<Color>();
  public ReactiveProperty<Color> suitsColor = new ReactiveProperty<Color>();
  public ReactiveProperty<Color> faceColor = new ReactiveProperty<Color>();
  public ReactiveProperty<Color> hairColor = new ReactiveProperty<Color>();
  public ReactiveProperty<Jobs> job = new ReactiveProperty<Jobs>();
  public ReactiveProperty<float> health =  new ReactiveProperty<float> ();  

  public ReactiveProperty<StaffNodePresenter> parentNode = new ReactiveProperty<StaffNodePresenter>();
  public ReactiveProperty<int?> parentDiff = new ReactiveProperty<int?>();

  CompositeDisposable childResources = new CompositeDisposable();
  protected CompositeDisposable eventResources = new CompositeDisposable();

  void Awake(){
    //define model

    childStream = childNodes.gameObject.OnTransformChildrenChangedAsObservable ();

    childCount = 
      childStream
        .Select (_ => childNodes.childCount)
        .ToReadOnlyReactiveProperty ();

    childCount
      .Subscribe (x => {
        childResources.Clear();
        foreach(Transform child in childNodes){
          NodePresenter node = child.GetComponent<NodePresenter>();
          node.childCountTotal
            .Subscribe(c => getChildTotal ())
            .AddTo(childResources);

          node.currentLevelTotal
            .Subscribe(c => getLevelTotal ())
            .AddTo(childResources);
        }
        getChildTotal();
        getLevelTotal();
      });

    currentLevel = baseLevel
      .CombineLatest (childCount, (s, c) =>  s - c )
      .ToReactiveProperty ();

    currentLevel
      .Subscribe (c => getLevelTotal ())
      .AddTo (eventResources);
    

    hasChild = 
      childCount
        .Select (c => 0 < c)
        .ToReadOnlyReactiveProperty ();
  }
  void getChildTotal(){
    int total = 0;
    foreach(Transform child in childNodes){
      total += child.GetComponent<NodePresenter> ().childCountTotal.Value;
    }
    childCountTotal.Value = total + 1;
  }
  void getLevelTotal(){
    int total = 0;
    foreach(Transform child in childNodes){
      total += child.GetComponent<NodePresenter> ().currentLevelTotal.Value;
    }
    currentLevelTotal.Value = total + currentLevel.Value;
  }

}
