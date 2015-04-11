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

  private float shirtsV = .8F;
  private float tieV = .3F;
  private float suitsV = .5F;

  public Dictionary<int?, StaffRxData> staffRxDataList = new Dictionary<int?, StaffRxData>();
  public Dictionary<int?, NodeRxData> nodeRxDataList = new Dictionary<int?, NodeRxData>();
  public Dictionary<int, StaffNodePresenter> nodeList = new Dictionary<int, StaffNodePresenter>();
  private int lastStaffId = 0;
  public ReactiveProperty<StaffNodePresenter> draggingNode = new ReactiveProperty<StaffNodePresenter> ();

  public const int retirementAge = 40;

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

    foreach (KeyValuePair<int, StaffNodePresenter> pair in nodeList) {
      addAge (pair.Value);
    }

    updateRecruits ();  

    GameSounds.accounting.Play ();
  }
  void addAge(StaffNodePresenter staff){
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
      destroyNode (child.gameObject);
    }
    int count = Random.Range (3, 7);
    for(int i = 0; i < count; i++){
      createStaffNode (createStaffData (), recruitContainer);
    }
  }
  private GameObject createStaffNode(StaffData staffData, Transform parentContainer, bool isHired = false, StaffNodePresenter parentNode = null){
    GameObject obj = Instantiate(staffNodePrefab) as GameObject;
    StaffNodePresenter node = obj.GetComponent<StaffNodePresenter> ();
    node.staffData = staffData;
    node.isHired.Value = isHired;
    if (parentNode) {
      node.parentNode.Value = parentNode;
      node.tier.Value = parentNode.tier.Value + 1;
    }
    obj.transform.SetParent (parentContainer);

    node.nodeId = lastStaffId;
    nodeList [lastStaffId] = node;
    lastStaffId++;
    return obj;
  }
  public void moveStaffNode(StaffNodePresenter node, NodePresenter parentNode = null){
    if (!parentNode) {
    }
    else if (parentNode is StaffNodePresenter) {
      moveStaffToStaff (node, parentNode as StaffNodePresenter);
    } else {
      //orgroot
      createStaffNode (node.staffData, parentNode.childNodes, true);
      GameSounds.promote.Play ();
    }
    node.isMoved = true;
    GameSounds.drop.Play();
  }
  private void moveStaffToStaff(StaffNodePresenter node, StaffNodePresenter parentStaff){
    int tierDiff = node.tier.Value - parentStaff.tier.Value;
    if (parentStaff.isAssigned.Value) {
      tierDiff -= 1;
      createStaffNode (node.staffData, parentStaff.childNodes, true, parentStaff);
    } else {
      if (0 == parentStaff.tier.Value) {
        tierDiff = 1;
      }
      parentStaff.staffData = node.staffData;
      parentStaff.isAssigned.Value = true;
      parentStaff.isMoved = false;
      parentStaff.gameObject.GetComponentInChildren<StaffNodeDragHandler> ().enabled = true;
    }
    if (0 < tierDiff) {
      GameSounds.promote.Play ();
    }
  }
  public void destroyNode(GameObject obj){
    nodeList.Remove (obj.GetComponent<StaffNodePresenter> ().nodeId);
    Destroy(obj);
  }
  public GameObject createStaffCursor(StaffData staffData){
    //clone
    GameObject cursor = createStaffNode (staffData, canvas.transform);
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
    data.baseSkill = data.lastSkill = Mathf.FloorToInt(baseSkill * .8f);
    data.age = age;

    float shirtsHue = Random.value;
    float tieHue = (.5F > Random.value) ? nearHue(shirtsHue) : compHue(shirtsHue);
    float suitsHue = (.5F > Random.value) ? nearHue(shirtsHue) : compHue(shirtsHue);

    data.shirtsColor = Util.HSVToRGB (shirtsHue, Random.value * .2F, shirtsV);
    data.tieColor = Util.HSVToRGB (tieHue, Random.value * .2F + .2F, tieV);
    data.suitsColor = Util.HSVToRGB (suitsHue, Random.value * .3F, suitsV);
    return data;
  }
  float nearHue(float hue){
    return (hue + Random.value * 1F/6F - 1F/12F + 1F ) % 1F;
  }
  float compHue(float hue){
    return (hue + .5F) % 1F;
  }

  // Update is called once per frame
  void Update () {

  }
}
