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
  private StaffNodePresenter node;
  private Image bg;
  private Image diffBg;
  private Text diffText;
  CompositeDisposable eventResources = new CompositeDisposable();



  void Start(){
    node = GetComponentInParent<StaffNodePresenter> ();
    bg = GetComponent<Image> ();
    diffBg = diffSkill.GetComponent<Image> ();
    diffText = diffSkill.GetComponentInChildren<Text> ();


    node.tier
      .CombineLatest(node.childCountStream, (t, c) => (0 < c) ? Mathf.Min(t, 2) : t)
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
            bg.color = new Color (1, 0, 0);
          } else if (diff.Value < 3) {
            bg.color = new Color (1, 1, Mathf.Pow(diff.Value/3f, .5f));
          } else {
            bg.color = new Color (1, 1, 1);
          }
        }else{
          bg.color = new Color (1, 1, 1);
        }
      })
      .AddTo(eventResources);    

    node.baseSkill
      .CombineLatest(node.childCountStream, (s, c) => 0 < c ? "/" + s : "" )
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
            diffBg.color = new Color(.9f,.9f,.5f);
            diffText.text = "+" + diff.ToString();
          }
          else{
            diffBg.color = new Color(.9f,.0f,.0f);
            diffText.text = diff.ToString();
          }
        }
      })
      .AddTo (eventResources);

    node.age
      .SubscribeToText(ageText, x => "(" + x.ToString() + ")" )
      .AddTo(eventResources);
    node.age
      .Subscribe (age => {
        ageText.color = (GameController.Instance.retirementAge > age) ? new Color (0, 0, 0) : new Color(1,0,0);
      })
      .AddTo (eventResources);
    node.age
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
