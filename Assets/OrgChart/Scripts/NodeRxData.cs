using UnityEngine;
using System.Collections;
using UniRx;
using System.Reflection;

public class NodeRxData {

  public ReadOnlyReactiveProperty<bool> hasChild { get; private set; }
  public ReadOnlyReactiveProperty<int> childCount { get; private set; }
  public ReadOnlyReactiveProperty<int> currentSkill { get; private set; }
  public ReadOnlyReactiveProperty<int> parentDiff { get; private set; }
  public ReactiveProperty<int> parentNodeId =  new ReactiveProperty<int>();  
  public ReactiveProperty<bool> isHired =  new ReactiveProperty<bool> (false);  
  public ReactiveProperty<bool> isAssigned =  new ReactiveProperty<bool> (false);  
  public ReactiveProperty<int> tier = new ReactiveProperty<int> (0);
  public ReactiveProperty<StaffRxData> staffData = new ReactiveProperty<StaffRxData> ();
  public ReactiveProperty<StaffNodePresenter> parentNode = new ReactiveProperty<StaffNodePresenter> ();

  public NodeRxData(StaffNodePresenter node){

//    childCount = 
//      node.childCountStream
//        .ToReadOnlyReactiveProperty ();

    staffData
      .Where (staff => staff != null)
      .Subscribe (staff => {
        //define prop
        currentSkill = 
          staff.baseSkill
            .CombineLatest (childCount, (s, c) => s - c)
            .ToReadOnlyReactiveProperty ();
    });

    parentNode
      .Where (parent => parent != null)
      .Subscribe (parent => {
    });




  }

}
