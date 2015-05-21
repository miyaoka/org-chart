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
  [SerializeField] RectTransform workingProjectContainer;
  [SerializeField] RectTransform planningProjectContainer;

  private RectTransform staffContainer;
  [SerializeField] GameObject staffNodePrefab;
  [SerializeField] GameObject projectPrefab;
  [SerializeField] Canvas canvas;

  public ReactiveProperty<bool> isDragging = new ReactiveProperty<bool> (false);

  private float shirtsV = .8F;
  private float tieV = .3F;
  private float suitsV = .5F;

  public Dictionary<int, StaffNodePresenter> nodeList = new Dictionary<int, StaffNodePresenter>();
  public Dictionary<int, ProjectPresenter> projectList = new Dictionary<int, ProjectPresenter>();


  public ReactiveProperty<StaffNodePresenter> draggingNode = new ReactiveProperty<StaffNodePresenter> ();

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
    SetupListeners();
    staffContainer = orgRoot.childNodes;
    foreach( Transform child in staffContainer){
      Destroy(child.gameObject);
    }
    updateRecruits ();
    updateProjects ();

    money.Value = 100;

    manPower = 
      orgRoot.currentLevelTotal.ToReactiveProperty ();

    manPower.SubscribeToText (manPowerText);
    money.SubscribeToText (moneyText);

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
    StaffNodePresenter[] nodes = new StaffNodePresenter [nodeList.Count];
    nodeList.Values.CopyTo (nodes, 0);
    foreach (StaffNodePresenter node in nodes) {
      addAge (node);
      if (node.isHired.Value && (.2f > UnityEngine.Random.value) ) {
        node.health.Value -= UnityEngine.Random.value * .5f;
      }
    }

    updateRecruits ();
    updateProjects ();

    money.Value -= manPower.Value;

    GameSounds.accounting.Play ();
  }
  void addAge(StaffNodePresenter staff){
    staff.age.Value++;
    staff.lastLevel.Value = staff.baseLevel.Value;
    staff.baseLevel.Value = growSkill(staff.age.Value, staff.baseLevel.Value);
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
    foreach( Transform t in planningProjectContainer){
      destroyProject (t.gameObject);
    }
    foreach( Transform t in workingProjectContainer){
      ProjectPresenter proj = t.GetComponent<ProjectPresenter>();

      proj.done.Value++;
      if (proj.done.Value >= proj.duration.Value) {
        money.Value += proj.reward.Value;
        destroyProject (t.gameObject);
      }
      /*
      if (proj.chance.Value > UnityEngine.Random.value) {
        money.Value += proj.reward.Value;
//        destroyProject (t.gameObject);
      }
      proj.reward.Value = (int)Mathf.Floor((float)proj.reward.Value * .9f);
      */
    }
    int count = UnityEngine.Random.Range (2, 4);
    for(int i = 0; i < count; i++){
      GameObject obj = createProject ();
      obj.transform.SetParent (planningProjectContainer);
    }
  }


  void updateRecruits(){
    foreach( Transform child in recruitContainer){
      destroyNode (child.gameObject);
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
    obj.transform.SetParent (parentContainer);

    nodeList [obj.GetInstanceID()] = node;
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
  public void destroyNode(GameObject obj){
    nodeList.Remove (obj.GetInstanceID());
    Destroy(obj);
  }
  private GameObject createProject(){
    GameObject obj = Instantiate(projectPrefab) as GameObject;
    ProjectPresenter proj = obj.GetComponent<ProjectPresenter> ();

    int id = obj.GetInstanceID();
    proj.title.Value = "proj" + id.ToString ();
    proj.manPower.Value = UnityEngine.Random.Range ((int)Mathf.Floor(manPower.Value * .2f) + 5, (int)Math.Max(20, manPower.Value)  );
    proj.chance.Value = UnityEngine.Random.value;
    proj.duration.Value = (int)System.Math.Ceiling (UDFs.BetaInv (UnityEngine.Random.value, .2d, 1d, 0, 0) * 5);
//    proj.reward.Value = (int)Mathf.Floor(proj.manPower.Value / proj.chance.Value * UnityEngine.Random.Range(1,3));
    proj.reward.Value = (int)Mathf.Floor(proj.manPower.Value * Mathf.Pow( (float) proj.duration.Value, 1.5f) * ( 1 + UnityEngine.Random.value * 2));

    proj.isSelected
      .Subscribe (v => proj.transform.SetParent (v ? workingProjectContainer : planningProjectContainer))
      .AddTo (proj);

    projectList [id] = proj;
    return obj;
  }
  public void destroyProject(GameObject obj){
    projectList.Remove (obj.GetInstanceID());
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
    int age = UnityEngine.Random.Range(0,35);

    age = (int)(UDFs.BetaInv (UnityEngine.Random.value, 2.0d, 1d, 0, 0) * 40);

    int baseSkill = UnityEngine.Random.Range(1,1);
    for(int i = 0; i < age; i++){
      baseSkill = growSkill (i, baseSkill);
    }
    data.baseLevel = data.lastLevel = Mathf.CeilToInt((float)baseSkill * .625f);
    data.age = age;

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
