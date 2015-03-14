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
	private RectTransform rect;
	float familyLineHeight = 38.0F;
	
	public int baseSkill = 10;
	public int age = 30;
	Vector3 lastPos;
	Vector3 lastParentPos;
		
	void Start(){
		rect = GetComponent<RectTransform>();
				
		baseSkill = 10;
		
		

		updateSkillText();
		updateFamilyLine();
		
//		SetupListeners();
	}
	
	void Update()  {
		if(familyLine ){
		/*
			Debug.Log("-----");
			Debug.Log (rect.position);
			Debug.Log (lastPos);
			Debug.Log (rect.parent.position);
			Debug.Log (lastParentPos);
*/			
			lastPos = rect.position;
			lastParentPos = rect.parent.position;
			updateFamilyLine();
			updateSkillText();
			transform.hasChanged = false;
		}
		
	}
	/*
	void OnDestroy(){
		DisposeListeners();
	}
	
	public void SetupListeners(){
		EventManager.Instance.AddListener<StaffRelationEvent>(OnStaffRelationUpdate);
	}
	
	public void DisposeListeners(){
//		if(EventManager.Instance){
			EventManager.Instance.RemoveListener<StaffRelationEvent>(OnStaffRelationUpdate);
//		}
	}
	
	public void OnStaffRelationUpdate(StaffRelationEvent evt){
//		updateSkillText();
//		updateFamilyLine();
	}	
	*/
	
	void updateSkillText(){
		currentSkillText.text = (baseSkill - chindrenContainer.childCount).ToString();
		baseSkillText.text = chindrenContainer.childCount == 0 ? "" : "/" + baseSkill.ToString();

		ageText.text = "(" + age.ToString() + ")";		
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
		if(!familyLine){
			return;
		}
		familyLine.enabled = true;
		
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
