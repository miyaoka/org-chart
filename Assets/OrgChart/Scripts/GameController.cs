using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UniRx;
using System;

public class GameController : MonoBehaviour {

  [SerializeField] RectTransform recruitContainer;
  [SerializeField] NodePresenter orgRoot;

  [SerializeField] GameObject staffNodePrefab;
  [SerializeField] Canvas canvas;
  [SerializeField] GameObject questPrefab;
  [SerializeField] RectTransform questContainer;

  public ReactiveProperty<bool> isDragging = new ReactiveProperty<bool> (false);


  public ReactiveProperty<bool> onQuest = new ReactiveProperty<bool> ();


  public ReactiveProperty<StaffNodePresenter> draggingNode = new ReactiveProperty<StaffNodePresenter> ();

  public ReactiveProperty<int> year = new ReactiveProperty<int> (1);
  public ReactiveProperty<int> money = new ReactiveProperty<int> ();
  public ReactiveProperty<int> manPower = new ReactiveProperty<int> ();

  public ReactiveProperty<QuestPresenter> selectedQuest = new ReactiveProperty<QuestPresenter> ();

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
  void Awake(){
    manPower = 
      orgRoot.currentLevelTotal.ToReactiveProperty ();
  }
  void Start () {

    startYear ();

    money.Value = 100;



  }
  public void nextPhase(){
    if (onQuest.Value) {
      endYear ();
    } else {
      startBattle ();
    }
    onQuest.Value = !onQuest.Value;
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



  void startYear(){
    updateRecruits ();
    updateProjects ();
  }

  void startPlan(){
  }
  void endPlan(){
  }
  void startBattle(){
    var q = selectedQuest.Value;
    if (!q) {
      endBattle ();
      return;
    }

    /*
    var staffs = new List<object>();
    orgRoot.GetComponentsInChildren<StaffNodePresenter> (staffs);


    var enemys = new List<object> { 0, 1, 2, 3 };

    var attacks = new List<object> ();
    attacks.AddRange (staffs);
    attacks.AddRange (enemys);

    attacks.Randomize ();
//    var ra = Util.shuffleArrayList (attacks);

    foreach(var r in attacks){
      Debug.Log (r.GetType());
    }
    */


  }



  void retreatBattle(){
  }
  void endBattle(){
  }

  void endYear(){
    StaffNodePresenter[] nodes = orgRoot.GetComponentsInChildren<StaffNodePresenter> ();
    foreach (StaffNodePresenter staff in nodes) {
      staff.age.Value++;
      staff.lastLevel.Value = staff.baseLevel.Value;
      staff.baseLevel.Value = growSkill(staff.age.Value, staff.baseLevel.Value);
    }
    year.Value++;
    startYear ();
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

  public void createQuest(float mp){
    GameObject obj = Instantiate(questPrefab) as GameObject;
    var q = obj.GetComponent<QuestPresenter> ();

    int id = obj.GetInstanceID();
    q.title.Value = "Quest " + id.ToString ();


    float healthFactor = 5f;
    float healthLevel = UnityEngine.Random.value;
    float attackLevel = UnityEngine.Random.value;
    float minHealth = 5f;
    float health = Mathf.Max (minHealth, Mathf.Ceil (Mathf.Pow (healthFactor, healthLevel - .5f) * mp));
    q.maxHealth.Value = health;
    q.health.Value = health * UnityEngine.Random.value;

    q.attack.Value = Mathf.Floor( attackLevel * mp );

    q.reward.Value = (int)Mathf.Floor(mp * (1f + healthLevel)  * ( 1f + UnityEngine.Random.value * 2f));



    q.transform.SetParent (questContainer);
  }

  void updateProjects(){
    foreach( Transform t in questContainer){
      var q = t.GetComponent<QuestPresenter> ();
      if(q == selectedQuest.Value) {

      } else {
        Destroy (t.gameObject);
      }
    }
    int count = UnityEngine.Random.Range (2, 4);
    for(int i = 0; i < count; i++){
      createQuest((float)manPower.Value);
      
    }


    /*
    ProjectManager.Instance.removePlanning ();
    int count = UnityEngine.Random.Range (2, 4);
    for(int i = 0; i < count; i++){
      ProjectManager.Instance.createProject ((float)manPower.Value);
    }
    */
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
