using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UniRx;

public class GameController : MonoBehaviour {

  [SerializeField] RectTransform recruitContainer;
  [SerializeField] RectTransform staffContainer;
  [SerializeField] GameObject staffNodePrefab;
  [SerializeField] Canvas canvas;

  public ReactiveProperty<bool> isDragging = new ReactiveProperty<bool> (false);

  private float shirtsV = .9F;
  private float tieV = .6F;
  private float suitsV = .6F;

  public Dictionary<int?, StaffRxData> staffRxDataList = new Dictionary<int?, StaffRxData>();
  private int lastStaffId = 0;
  public ReactiveProperty<StaffNodePresenter> draggingNode = new ReactiveProperty<StaffNodePresenter> ();

  public int retirementAge = 60;

  private static GameController s_Instance;
  public static GameController Instance {
    get {
      if (s_Instance == null) {
        s_Instance = GameObject.FindObjectOfType (typeof(GameController)) as GameController;
      }
      return s_Instance;
    }
  }


  // Use this for initialization
  void Start () {
    SetupListeners();
    foreach( Transform child in staffContainer){
      Destroy(child.gameObject);
    }
    updateRecruits ();
  }
  public void SetupListeners(){
//    EventManager.Instance.AddListener<ChartChangeEvent>(onChartChange);
    EventManager.Instance.AddListener<EndTurnEvent>(onEndTurn);
  }
  void OnDestroy(){
    DisposeListeners();
  }
  public void DisposeListeners(){
    if(EventManager.Instance){
//      EventManager.Instance.RemoveListener<ChartChangeEvent>(onChartChange);
      EventManager.Instance.RemoveListener<EndTurnEvent>(onEndTurn);
    }
  }
  void onEndTurn(EndTurnEvent e){

    foreach (KeyValuePair<int?, StaffRxData> pair in staffRxDataList) {
      addAge (pair.Value);
    }

    updateRecruits ();  
  }
  void addAge(StaffRxData staff){
    staff.age.Value++;
    staff.lastSkill.Value = staff.baseSkill.Value;
    if (retirementAge > staff.age.Value) {
      if (.5 > Random.value) {
        staff.baseSkill.Value++;
      }
    } else {
      if (.5 > Random.value) {
        staff.baseSkill.Value-=2;
      } else {
        staff.baseSkill.Value--;
      }
    }
  }


  void updateRecruits(){
    foreach( Transform child in recruitContainer){
      Destroy(child.gameObject);
    }
    int count = Random.Range (3, 7);
    for(int i = 0; i < count; i++){
      createStaff();
    }

  }
  GameObject createStaff(){
    StaffData data = createStaffData ();
    StaffRxData srd = new StaffRxData ();
    srd.staffData = data;
    staffRxDataList [lastStaffId] = srd;

    GameObject staffNode = createStaffNode (lastStaffId, recruitContainer);

    lastStaffId++;
    return staffNode;
  }
  private GameObject createStaffNode(int? staffId, Transform parentContainer, bool isHired = false, StaffNodePresenter parentStaff = null){
    GameObject staffNode = Instantiate(staffNodePrefab) as GameObject;
    StaffNodePresenter node = staffNode.GetComponent<StaffNodePresenter> ();
    node.staffId.Value = staffId;
    node.isHired.Value = isHired;
    if (parentStaff) {
      node.parentId.Value = parentStaff.staffId.Value;
      node.tier.Value = parentStaff.tier.Value + 1;
    }
    staffNode.transform.SetParent (parentContainer);
    return staffNode;
  }
  public void moveStaffNode(StaffNodePresenter staff, NodePresenter parentNode){
    createStaffNode (staff.staffId.Value, parentNode.childNodes, true, 
      parentNode is StaffNodePresenter ? parentNode as StaffNodePresenter : null);
    Destroy (staff.gameObject);
    GameSounds.auDrop.Play();
  }
  public void destroyNode(StaffNodePresenter snp){
    
  }
  public GameObject createStaffCursor(int? staffId){

    //clone
    GameObject cursor = createStaffNode (staffId, canvas.transform);
    //add to canvas
    cursor.transform.SetAsLastSibling();

    //set size
    cursor.GetComponent<ContentSizeFitter>().enabled = true;

    //set canvas
    CanvasGroup dcg = cursor.GetComponent<CanvasGroup>();
    dcg.blocksRaycasts = false;
    dcg.alpha = .75F; 


    return cursor;

  }

  StaffData createStaffData(){
    StaffData data = new StaffData ();
    int age = Random.Range(0,40);
    float baseSkill = 0;
    for(int i = 0; i < age; i++){
      if(.5F <= Random.value){
        baseSkill++;
      }
    }
    data.baseSkill = 
      data.lastSkill = Mathf.FloorToInt(baseSkill * .8F);
    data.age = age + 20;

    float shirtsHue = Random.value;
    float tieHue = (.5F > Random.value) ? nearHue(shirtsHue) : compHue(shirtsHue);
    float suitsHue = (.5F > Random.value) ? nearHue(shirtsHue) : compHue(shirtsHue);

    data.shirtsColor = HSVToRGB (shirtsHue, Random.value * .2F, shirtsV);
    data.tieColor = HSVToRGB (tieHue, Random.value * .2F + .2F, tieV);
    data.suitsColor = HSVToRGB (suitsHue, Random.value * .3F, suitsV);

    return data;
  }
  float nearHue(float hue){
    return (hue + Random.value * 1F/6F - 1F/12F + 1F ) % 1F;
  }
  float compHue(float hue){
    return (hue + .5F) % 1F;
  }
  public static Color HSVToRGB(float H, float S, float V)
  {
    if (S == 0f) {
      return new Color (V, V, V);
    }
    else if (V == 0f){
      return Color.black;
    }
    else
    {
      Color col = Color.black;
      float Hval = H * 6f;
      int sel = Mathf.FloorToInt(Hval);
      float mod = Hval - sel;
      float v1 = V * (1f - S);
      float v2 = V * (1f - S * mod);
      float v3 = V * (1f - S * (1f - mod));
      switch (sel + 1)
      {
      case 0:
        col.r = V;
        col.g = v1;
        col.b = v2;
        break;
      case 1:
        col.r = V;
        col.g = v3;
        col.b = v1;
        break;
      case 2:
        col.r = v2;
        col.g = V;
        col.b = v1;
        break;
      case 3:
        col.r = v1;
        col.g = V;
        col.b = v3;
        break;
      case 4:
        col.r = v1;
        col.g = v2;
        col.b = V;
        break;
      case 5:
        col.r = v3;
        col.g = v1;
        col.b = V;
        break;
      case 6:
        col.r = V;
        col.g = v1;
        col.b = v2;
        break;
      case 7:
        col.r = V;
        col.g = v3;
        col.b = v1;
        break;
      }
      col.r = Mathf.Clamp(col.r, 0f, 1f);
      col.g = Mathf.Clamp(col.g, 0f, 1f);
      col.b = Mathf.Clamp(col.b, 0f, 1f);
      return col;
    }
  }
  // Update is called once per frame
  void Update () {

  }
}
