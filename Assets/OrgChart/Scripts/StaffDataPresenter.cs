using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class StaffDataPresenter : MonoBehaviour {
  //view
  [SerializeField] Text currentSkillText;
  [SerializeField] Text baseSkillText;
  [SerializeField] GameObject diffSkill;
  [SerializeField] Text ageText;

  [SerializeField] Image shirts;
  [SerializeField] Image tie;
  [SerializeField] Image suits;
  [SerializeField] Image face;
  [SerializeField] Image hair;
  [SerializeField] HairSprites hairPrefab;

  //model
  public ReadOnlyReactiveProperty<int> currentSkill { get; private set; }
  public ReactiveProperty<int?> parentSkill = new ReactiveProperty<int?> ();

  CompositeDisposable eventResources = new CompositeDisposable();
  CompositeDisposable staffResources = new CompositeDisposable();


  private StaffNodePresenter node;
  private Image bg;
  private Image diffBg;
  private Text diffText;

  void Start(){
    node = GetComponentInParent<StaffNodePresenter> ();
    bg = GetComponent<Image> ();
    diffBg = diffSkill.GetComponent<Image> ();
    diffText = diffSkill.GetComponentInChildren<Text> ();

    node.staffId
      .Where(id => id.HasValue)
      .Subscribe(id => bindToView (GameController.Instance.staffRxDataList [id.Value]))
      .AddTo(eventResources);


    node.tier
      .CombineLatest(node.childCountStream, (t, c) => (0 < c) ? Mathf.Min(t, 2) : t)
      .Subscribe(t => {
        if(t < 2){
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

  }

  void bindToView(StaffRxData srd){
    staffResources.Dispose ();
    staffResources = new CompositeDisposable();

    //define prop
    currentSkill = 
      srd.baseSkill
        .CombineLatest(node.childCountStream, (s, c) => s - c )
        .ToReadOnlyReactiveProperty();

    //model to view
    currentSkill
      .SubscribeToText(currentSkillText)
      .AddTo(staffResources);
    currentSkill
      .CombineLatest (parentSkill, (c, p) => p.HasValue ? p.Value - c : 10)
      .Subscribe (diff => {
        if(0 > diff){
          bg.color = new Color(1,0,0);
        }
        else if(0 == diff){
          bg.color = new Color(1,1,0);
        }
        else if(1 == diff){
          bg.color = new Color(1,1,.75f);
        }
        else {
          bg.color = new Color(1,1,1);
        }
      })
      .AddTo(staffResources);


    srd.baseSkill
      .CombineLatest(node.childCountStream, (s, c) => 0 < c ? "/" + s : "" )
      .SubscribeToText(baseSkillText)
      .AddTo(staffResources);
    srd.baseSkill
      .CombineLatest (srd.lastSkill, (b, l) => b - l)
      .Subscribe (diff => {
        if(0 == diff){
          diffSkill.gameObject.SetActive(false);
        }
        else{
          diffSkill.gameObject.SetActive(true);
          if(0 < diff){
            diffBg.color = new Color(.9f,.9f,.5f);
            diffText.text = "+" + diff.ToString();
          }
          else{
            diffBg.color = new Color(.9f,.0f,.0f);
            diffText.text = diff.ToString();
          }
        }
      })
      .AddTo (staffResources);
    
    srd.age
      .SubscribeToText(ageText, x => "(" + x.ToString() + ")" )
      .AddTo(staffResources);
    srd.age
      .Subscribe (age => {
        ageText.color = (GameController.Instance.retirementAge > age) ? new Color (0, 0, 0) : new Color(1,0,0);
      })
      .AddTo (staffResources);
    srd.age
      .Subscribe(x => hair.sprite = hairPrefab.hairByAge(x) )
      .AddTo(staffResources);
    srd.shirtsColor
      .Subscribe(x => shirts.color = x)
      .AddTo(staffResources);
    srd.tieColor
      .Subscribe(x => tie.color = x)
      .AddTo(staffResources);
    srd.suitsColor
      .Subscribe(x => suits.color = x)
      .AddTo(staffResources);

  }
  void OnDestroy()
  {
    eventResources.Dispose();
    staffResources.Dispose ();
  }



}
