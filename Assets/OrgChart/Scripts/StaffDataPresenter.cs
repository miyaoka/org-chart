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
//  [SerializeField] Image relation;

  //model
  private StaffNodePresenter node;
  private Image relation;
  private Image diffBg;
  private Text diffText;
  CompositeDisposable eventResources = new CompositeDisposable();



  void Start(){
    node = GetComponentInParent<StaffNodePresenter> ();
    relation = GetComponent<Image> ();
    diffBg = diffSkill.GetComponent<Image> ();
    diffText = diffSkill.GetComponentInChildren<Text> ();


    node.tier
      .CombineLatest(node.childCount, (t, c) => (0 < c) ? Mathf.Min(t, 2) : t)
      .Subscribe(t => {
        if(node.isHired.Value){
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
        }
        else{
          tie.enabled = false;
          suits.enabled = true;
        }
      })
      .AddTo(eventResources);

    node.currentSkill
      .SubscribeToText(currentSkillText)
      .AddTo(eventResources);
    node.parentDiff
      .Subscribe(diff => {
        if(diff.HasValue)
        {
          if (diff.Value < 0) {
            relation.color = new Color (1, 0, 0);
          } else if (diff.Value < 3) {
            relation.color = new Color (1, 1, Mathf.Pow(diff.Value/3f, .2f));
          } else {
            relation.color = new Color (1, 1, 1);
          }
        }else{
          relation.color = new Color (1, 1, 1);
        }
      })
      .AddTo(eventResources);    

    node.baseSkill
      .CombineLatest(node.hasChild, (s, c) => c ? "/" + s : "" )
      .SubscribeToText(baseSkillText)
      .AddTo(eventResources);
    node.baseSkill
      .CombineLatest (node.lastSkill, (b, l) => b - l)
      .Subscribe (diff => {
        if(0 == diff){
          diffSkill.gameObject.SetActive(false);
        }
        else{
          diffSkill.gameObject.SetActive(true);
          if(0 < diff){
            diffBg.color = new Color(.1f,.5f,.2f);
            diffText.text = "+" + diff.ToString();
          }
          else{
            diffBg.color = new Color(.9f,.0f,.0f);
            diffText.text = diff.ToString();
          }
        }
      })
      .AddTo (eventResources);
    node.baseSkill
      .CombineLatest(node.age, (skill,age) => age == 0 ? .5f : Mathf.Min(1, (float)skill/age/.8f))
      .Subscribe (rate => {
        currentSkillText.color = Util.HSVToRGB(rate * 100f/360f, .9f, .7f);
    });

    node.age
      .Select(age => age + 20)
      .SubscribeToText(ageText, age => "(" + age.ToString() + ")" )
      .AddTo(eventResources);
    node.age
      .Subscribe (age => {
        if(age < GameController.retirementAge)
        {
          ageText.color =  Util.HSVToRGB(.3f, 1, (1f - (float)age / GameController.retirementAge) * .6f );
        } else{
          ageText.color = new Color(1,0,0);
        }
      })
      .AddTo (eventResources);
    node.age
      .Select(age => age + 20)
      .Subscribe(x => hair.sprite = hairPrefab.hairByAge(x) )
      .AddTo(eventResources);
    node.shirtsColor
      .Subscribe(x => shirts.color = x)
      .AddTo(eventResources);
    node.tieColor
      .Subscribe(x => tie.color = x)
      .AddTo(eventResources);
    node.suitsColor
      .Subscribe(x => suits.color = x)
      .AddTo(eventResources);
  }
  void OnDestroy()
  {
    eventResources.Dispose ();
  }
}
