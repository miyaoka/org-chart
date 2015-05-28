using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class StaffDataPresenter : MonoBehaviour {
  //view
  [SerializeField] Text currentLevelText;
  [SerializeField] Text baseLevelText;
  [SerializeField] GameObject diffLevelUI;
  [SerializeField] Text ageText;
  [SerializeField] RectTransform healthUI;
  [SerializeField] Text nameText;

  [SerializeField] Image shirts;
  [SerializeField] Image tie;
  [SerializeField] Image suits;
  [SerializeField] Image face;
  [SerializeField] Image hair;
  [SerializeField] HairSprites hairPrefab;
//  [SerializeField] Image relation;
  [SerializeField] GameObject researchUI;
  [SerializeField] GameObject developUI;
  [SerializeField] GameObject marketUI;

  //model
  private StaffNodePresenter node;
  private Image relation;
  private Image diffBg;
  private Text diffText;
  CompositeDisposable eventResources = new CompositeDisposable();



  void Start(){
    node = GetComponentInParent<StaffNodePresenter> ();
    relation = GetComponent<Image> ();
    diffBg = diffLevelUI.GetComponent<Image> ();
    diffText = diffLevelUI.GetComponentInChildren<Text> ();

    node.health
      .Subscribe (h => {
        healthUI.sizeDelta = new Vector2 (8, Mathf.Ceil(52f * h));
        healthUI.GetComponent<Image>().color = Util.HSVToRGB(h * 100f/360f, 1f, 1f);
      })
      .AddTo (eventResources);


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

    node.currentLevel
      .SubscribeToText(currentLevelText)
      .AddTo(eventResources);
    node.parentDiff
      .Subscribe(diff => {
        if(diff.HasValue)
        {
          if (diff.Value < 0) {
            relation.color = new Color (1, 0, 0);
          } else if (diff.Value < 2) {
            relation.color = new Color (1, 1, Mathf.Pow(diff.Value/2f, .8f));
          } else {
            relation.color = new Color (1, 1, 1);
          }
        }else{
          relation.color = new Color (1, 1, 1);
        }
      })
      .AddTo(eventResources);    

    node.baseLevel
      .CombineLatest(node.hasChild, (s, c) => c ? "/" + s : "" )
      .SubscribeToText(baseLevelText)
      .AddTo(eventResources);
    node.baseLevel
      .CombineLatest (node.lastLevel, (b, l) => b - l)
      .Subscribe (diff => {
        if(0 == diff){
          diffLevelUI.gameObject.SetActive(false);
        }
        else{
          diffLevelUI.gameObject.SetActive(true);
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
    node.baseLevel
      .CombineLatest(node.age, (skill,age) => age == 0 ? .5f : Mathf.Min(1, (float)skill/age/.8f))
      .Subscribe (rate => {
        currentLevelText.color = Util.HSVToRGB(rate * 100f/360f, .9f, .7f);
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

    node.name
      .SubscribeToText (nameText)
      .AddTo (eventResources);

    node.gender
      .Subscribe (g => {
        nameText.color = ((g == 0) ? new Color(1f, .8f, .8f) : new Color(.9f, .9f, 1f));
    })
      .AddTo (eventResources);
    /*
    node.shirtsColor
      .Subscribe(x => shirts.color = x)
      .AddTo(eventResources);
    node.tieColor
      .Subscribe(x => tie.color = x)
      .AddTo(eventResources);
    node.suitsColor
      .Subscribe(x => suits.color = x)
      .AddTo(eventResources);

    node.job
      .Subscribe(j => {
        researchUI.SetActive(j == Jobs.Research);
        developUI.SetActive(j == Jobs.Develop);
        marketUI.SetActive(j == Jobs.Market);
      })
      .AddTo(eventResources);
    */

  }
  void OnDestroy()
  {
    eventResources.Dispose ();
  }
}
