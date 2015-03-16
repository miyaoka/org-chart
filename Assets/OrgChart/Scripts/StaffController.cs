using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StaffController : MonoBehaviour {
	
	[SerializeField] RectTransform chindrenContainer;
	[SerializeField] Text currentSkillText;
	[SerializeField] Text baseSkillText;
	[SerializeField] Text ageText;
	[SerializeField] UILineRenderer familyLine;
	
	[SerializeField] Image shirts;
	[SerializeField] Image tie;
	[SerializeField] Image suits;
	[SerializeField] Image face;
	[SerializeField] Image hair;
	
	[SerializeField] HairSprites hairSprites;
	
	
	float familyLineHeight = 19.0F;
	
	int baseSkill = 10;
	int age = 30;
	int tier = 0;
	Vector3 lastPos;
	Vector3 lastParentPos;
		
	void Start(){
//		updateSkillText();
//		updateFamilyLine();
		
		SetupListeners();
	}
	
	void Update()  {
		if(familyLine.enabled){
		/*
			Debug.Log("-----");
			Debug.Log (rect.position);
			Debug.Log (lastPos);
			Debug.Log (rect.parent.position);
			Debug.Log (lastParentPos);
*/			
//			lastPos = rect.position;
//			lastParentPos = rect.parent.position;
//			transform.hasChanged = false;
		}
		
	}


	public void setData(StaffData data){
		age = data.age;
		baseSkill = data.baseSkill;
		shirts.color = data.shirtsColor;
		tie.color = data.tieColor;
		suits.color = data.suitsColor;


		updateInfo();
		
	}
	public void SetupListeners(){
		EventManager.Instance.AddListener<ChartChangeEvent>(OnChartChange);
	}
	void OnDestroy(){
		DisposeListeners();
	}
	public void DisposeListeners(){
		if(EventManager.Instance){
			EventManager.Instance.RemoveListener<ChartChangeEvent>(OnChartChange);
		}
	}
	public void OnChartChange(ChartChangeEvent evt){
		StartCoroutine (updateFamilyTreeOnNextFrame() );
//		Invoke ("updateFamilyLine", 1/30F);
//		updateFamilyLine();
		updateInfo();
	}
	IEnumerator updateFamilyTreeOnNextFrame(){
		familyLine.enabled = false;

		//returning 0 will make it wait 1 frame
		yield return 0;

		updateFamilyLine();
	}

//	public StaffData getData(){
	
//	}
	/*


	

	public void OnStaffRelationUpdate(StaffRelationEvent evt){
//		updateSkillText();
//		updateFamilyLine();
	}	
	*/
	
	void updateInfo(){
		int childCount = chindrenContainer.childCount;
		currentSkillText.text = (baseSkill - childCount).ToString();
		baseSkillText.text = childCount == 0 ? "" : "/" + baseSkill.ToString();

		if (childCount == 0) {
//			tie.enabled = false;
			suits.enabled = false;
		} else {
			tie.enabled = true;
			suits.enabled = true;
		}

		ageText.text = "(" + age.ToString() + ")";
		updateHair();
	}
	void updateHair(){
		hair.sprite = hairSprites.hairByAge(age);
//		Debug.Log (hairSprites.hairByAge(age));
			//hair.sprite = 
//		Debug.Log ("hair2" + Mathf.FloorToInt((float)age / 10));
//		Debug.Log(this.GetType().GetField("hair" + Mathf.FloorToInt((float)age / 10)).GetValue(this));
		
		
	}
	/*
	public void update()
	{
		RectTransform parentRect = rect.parent.GetComponent<RectTransform>();
//		familyLine.enabled = false;
		
		return;
		Debug.Log("update");
		
		Debug.Log(transform);
		Debug.Log(transform.position);
		Debug.Log(parentRect.position);
		
//		updateFamilyLine();
	}
	*/
	public void updateFamilyLine(){
		if (tier == 1) {
			familyLine.enabled = false;
		}
		familyLine.enabled = true;

		RectTransform rect = GetComponent<RectTransform>();	
		RectTransform parentRect = rect.parent.GetComponent<RectTransform>();
		
		float startX = rect.sizeDelta.x/2;
		float endX = parentRect.sizeDelta.x/2 - rect.position.x + parentRect.position.x;
		familyLine.Points = new Vector2[] { 
			new Vector2(startX, 0), 
			new Vector2(startX, 6),//familyLineHeight * .4F),
			new Vector2(endX, 6),//familyLineHeight * .4F),
			new Vector2(endX, familyLineHeight * .6F),
			
			new Vector2(endX, familyLineHeight)
		};
//		familyLine.color = new Color(Random.Range(0,1.0F), Random.Range(0,1.0F),Random.Range(0,1.0F));
		familyLine.SetVerticesDirty();		
	}
	
	
	
}
