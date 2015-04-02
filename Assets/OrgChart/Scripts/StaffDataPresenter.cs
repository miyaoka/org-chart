using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;
using Unity.Linq;
using UnityEngine.EventSystems;
using System.Reflection;
using System;

public class StaffDataPresenter : MonoBehaviour {
  //view
  [SerializeField] Text currentSkillText;
  [SerializeField] Text baseSkillText;
  [SerializeField] Text ageText;

  [SerializeField] Image shirts;
  [SerializeField] Image tie;
  [SerializeField] Image suits;
  [SerializeField] Image face;
  [SerializeField] Image hair;
  [SerializeField] HairSprites hairPrefab;

  //model
  public ReadOnlyReactiveProperty<int> currentSkill { get; private set; }

  public IntReactiveProperty baseSkill =  new IntReactiveProperty();  
  public IntReactiveProperty age = new IntReactiveProperty();
  public ReactiveProperty<Color> shirtsColor = new ReactiveProperty<Color>();
  public ReactiveProperty<Color> tieColor = new ReactiveProperty<Color>();
  public ReactiveProperty<Color> suitsColor = new ReactiveProperty<Color>();
  public ReactiveProperty<Color> faceColor = new ReactiveProperty<Color>();
  public ReactiveProperty<Color> hairColor = new ReactiveProperty<Color>();


  CompositeDisposable eventResources = new CompositeDisposable();

  public GameObject dragPointer;


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


  void Start(){
    StaffNodePresenter node = GetComponentInParent<StaffNodePresenter> ();

    //define prop
    currentSkill = 
      baseSkill
        .CombineLatest(node.childCountStream, (s, c) => s - c )
        .ToReadOnlyReactiveProperty();

    //model to view
    currentSkill
      .SubscribeToText(currentSkillText)
      .AddTo(eventResources);

    baseSkill
      .CombineLatest(node.childCountStream, (s, c) => 0 < c ? "/" + s : "" )
      .SubscribeToText(baseSkillText)
      .AddTo(eventResources);
    age
      .SubscribeToText(ageText, x => "(" + x.ToString() + ")" )
      .AddTo(eventResources);
    age
      .Subscribe(x => hair.sprite = hairPrefab.hairByAge(x) );
    shirtsColor
      .Subscribe(x => shirts.color = x);
    tieColor
      .Subscribe(x => tie.color = x);
    suitsColor
      .Subscribe(x => suits.color = x);

    /*
    node.tier
      .CombineLatest(node.childCountStream, (t, c) => (0 < c) ? Mathf.Min(t, 1) : t)
      .Subscribe(t => {
        if(t == 1){
          tie.enabled = true;
          suits.enabled = true;
        } else if(t == 2){
          tie.enabled = true;
          suits.enabled = false;      
        } else {
          tie.enabled = false;
          suits.enabled = false;            
        }
      })
      .AddTo(eventResources);

*/
  }
  void OnDestroy()
  {
    eventResources.Dispose();
  }



}
