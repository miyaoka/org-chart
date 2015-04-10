using UnityEngine;
using System.Collections;
using UniRx;
using System.Reflection;

public class StaffRxData {
//  public ReadOnlyReactiveProperty<int> currentSkill { get; private set; }
  public ReactiveProperty<int> baseSkill =  new ReactiveProperty<int> ();  
  public ReactiveProperty<int> lastSkill =  new ReactiveProperty<int> ();  
  public ReactiveProperty<int> age = new ReactiveProperty<int> ();
  public ReactiveProperty<Color> shirtsColor = new ReactiveProperty<Color>();
  public ReactiveProperty<Color> tieColor = new ReactiveProperty<Color>();
  public ReactiveProperty<Color> suitsColor = new ReactiveProperty<Color>();
  public ReactiveProperty<Color> faceColor = new ReactiveProperty<Color>();
  public ReactiveProperty<Color> hairColor = new ReactiveProperty<Color>();

  public ReactiveProperty<int?> parentStaffId =  new ReactiveProperty<int?> ();  
  public ReactiveProperty<bool> isHired =  new ReactiveProperty<bool> ();  
  public ReactiveProperty<int> tier = new ReactiveProperty<int> ();

  public StaffData staffData{
    get {
      var sd = new StaffData();        
      foreach(FieldInfo fi in sd.GetType().GetFields()){
        object reactiveProp = this.GetType ().GetField (fi.Name).GetValue (this);
        sd.GetType().GetField(fi.Name).SetValue(
          sd, 
          reactiveProp.GetType ().GetProperty ("Value").GetValue(reactiveProp, null)
        );
      }
      return sd;
    }
    set {
      foreach(FieldInfo fi in value.GetType().GetFields()){
        object reactiveProp = this.GetType ().GetField (fi.Name).GetValue (this);
        reactiveProp.GetType ().GetProperty ("Value").SetValue(
          reactiveProp,
          value.GetType ().GetField (fi.Name).GetValue (value),
          null
        );
      }
    }
  }
}
