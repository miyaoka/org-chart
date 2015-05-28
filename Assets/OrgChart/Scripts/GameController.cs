using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UniRx;
using System;

public class GameController : MonoBehaviour {

  [SerializeField] RectTransform recruitContainer;
  [SerializeField] NodePresenter orgRoot;
  [SerializeField] Text manPowerText;
  [SerializeField] Text moneyText;

  [SerializeField] GameObject staffNodePrefab;
  [SerializeField] Canvas canvas;

  public ReactiveProperty<bool> isDragging = new ReactiveProperty<bool> (false);

  private float shirtsV = .8F;
  private float tieV = .3F;
  private float suitsV = .5F;

  public ReactiveProperty<bool> onQuest = new ReactiveProperty<bool> ();


  public ReactiveProperty<StaffNodePresenter> draggingNode = new ReactiveProperty<StaffNodePresenter> ();

  public ReactiveProperty<int> year = new ReactiveProperty<int> (0);
  public ReactiveProperty<int> money = new ReactiveProperty<int> ();
  public ReactiveProperty<int> manPower = new ReactiveProperty<int> ();

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

    startYear ();

    money.Value = 100;

    manPower = 
      orgRoot.currentLevelTotal.ToReactiveProperty ();

    manPower.SubscribeToText (manPowerText);
    money.SubscribeToText (moneyText);

  }
  public void nextPhase(){
    if (onQuest.Value) {
      startYear ();
    } else {
      doPlan ();
      showResult ();
      endYear ();
    }
    onQuest.Value = !onQuest.Value;
  }
  void setOnPhasePlan(){
  }
  void setOnPhaseDo(){


  }
  void doPlan(){
    var staffs = new List<StaffNodePresenter> ();
    orgRoot.GetComponentsInChildren<StaffNodePresenter> (staffs);

    var wlist = ProjectManager.Instance.workingProject;
    if (wlist.Count < 1) {
      return;
    }
    var quest = wlist [0];


    foreach (StaffNodePresenter s in staffs) {
      if (.5f > UnityEngine.Random.value) {
        quest.health.Value -= s.currentLevel.Value;
        
      }
    }
    /*


    foreach( Transform t in workingProjectContainer){
      ProjectPresenter proj = t.GetComponent<ProjectPresenter>();

      if (0 >= proj.health.Value) {
        money.Value += proj.reward.Value;
        Destroy (t.gameObject);
      }
    }
*/
  }

  void showResult(){
  }

  void startYear(){
    year.Value++;
    updateRecruits ();
    updateProjects ();
  }
  void endYear(){
    StaffNodePresenter[] nodes = orgRoot.GetComponentsInChildren<StaffNodePresenter> ();
    foreach (StaffNodePresenter staff in nodes) {
      staff.age.Value++;
      staff.lastLevel.Value = staff.baseLevel.Value;
      staff.baseLevel.Value = growSkill(staff.age.Value, staff.baseLevel.Value);
    }
  }


  void onEndTurn(EndTurnEvent e){
    

    updateRecruits ();
    updateProjects ();

    money.Value -= manPower.Value;

    GameSounds.accounting.Play ();
  }
  int growSkill(int age, int skill){
    
    if (retirementAge > age) {
      if (.4 > UnityEngine.Random.value) {
        skill++;
      }
    } else {
      if (.6 > UnityEngine.Random.value) {
        skill-=1;
      } else {
      }
    }
    return skill;
  }

  void updateProjects(){
    ProjectManager.Instance.removePlanning ();
    int count = UnityEngine.Random.Range (2, 4);
    for(int i = 0; i < count; i++){
      ProjectManager.Instance.createProject ((float)manPower.Value);
    }
  }
  void updateRecruits(){
    foreach( Transform t in recruitContainer){
      Destroy (t.gameObject);
    }
    int count = UnityEngine.Random.Range (3, 4);
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
    obj.transform.SetParent (parentContainer, false);

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
    node.health.Value = 1.0f;
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
    int age = UnityEngine.Random.Range(0,35);

    age = (int)(UDFs.BetaInv (UnityEngine.Random.value, 1.4d, 1d, 0, 0) * 40);

    int baseSkill = UnityEngine.Random.Range(1,1);
    for(int i = 0; i < age; i++){
      baseSkill = growSkill (i, baseSkill);
    }
    data.baseLevel = data.lastLevel = Mathf.CeilToInt((float)baseSkill * .625f);
    data.age = age;
    data.gender = (.2f > UnityEngine.Random.value) ? 0 : 1;
    data.name = Names.getRandomName (data.gender);

    float shirtsHue = UnityEngine.Random.value;
    float tieHue = (.5F > UnityEngine.Random.value) ? nearHue(shirtsHue) : compHue(shirtsHue);
    float suitsHue = (.5F > UnityEngine.Random.value) ? nearHue(shirtsHue) : compHue(shirtsHue);

    data.shirtsColor = Util.HSVToRGB (shirtsHue, UnityEngine.Random.value * .2F, shirtsV);
    data.tieColor = Util.HSVToRGB (tieHue, UnityEngine.Random.value * .2F + .2F, tieV);
    data.suitsColor = Util.HSVToRGB (suitsHue, UnityEngine.Random.value * .3F, suitsV);

    data.job = (Jobs)Enum.ToObject( typeof(Jobs), UnityEngine.Random.Range (0, Enum.GetValues (typeof(Jobs)).Length));
    return data;
  }
  float nearHue(float hue){
    return (hue + UnityEngine.Random.value * 1F/6F - 1F/12F + 1F ) % 1F;
  }
  float compHue(float hue){
    return (hue + .5F) % 1F;
  }

  // Update is called once per frame
  void Update () {

  }
}
