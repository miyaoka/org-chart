using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StaffController : MonoBehaviour {
	
	[SerializeField] RectTransform childrenContainer;
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
	[SerializeField] GameObject staffPrefab;
	
    
    float familyLineHeight = 19.0F;
	
//	string[] fields = "age baseSkill tier".Split(" "[0]);
	public int age = 30;
    public int baseSkill = 10;
	public int tier = 0;

		
	void Start(){	
	}
	
	void Update()  {
	}
	public StaffData data{
		get{
			StaffData sd = new StaffData();
			
			sd.age = age;
			sd.baseSkill = baseSkill;
			sd.tier = tier;
			sd.shirtsColor = shirts.color;
			sd.tieColor = tie.color;
			sd.suitsColor = suits.color;
			sd.children = new StaffData[childrenContainer.childCount];
			for(int i = 0; i < childrenContainer.childCount; i++){
				sd.children[i] = childrenContainer.GetChild(i).GetComponent<StaffController>().data;
			}
			
			return sd;
		
		}
		set{
			age = value.age;
			baseSkill = value.baseSkill;
			tier = value.tier;

			shirts.color = value.shirtsColor;
			tie.color = value.tieColor;
			suits.color = value.suitsColor;
			
			foreach(Transform child in childrenContainer){
				Destroy(child.gameObject);		
			}
			for(int i=0; i < value.children.Length; i++){
				GameObject child = Instantiate(staffPrefab) as GameObject;
				child.GetComponent<StaffController>().data = value.children[i];
				/*
				Debug.Log ("---");
				Debug.Log (value.children[i].children.Length);
				Debug.Log (child.GetComponent<StaffController>().childrenContainer.childCount);
				*/
				child.transform.SetParent(childrenContainer);
			}
            updateInfo();
            updateFamilyLine();
		}
	}


	IEnumerator updateFamilyTreeOnNextFrame(){
		familyLine.enabled = false;

		//returning 0 will make it wait 1 frame
		yield return 0;

		updateFamilyLine();
	}

	
	void updateInfo(){
		int childCount = childrenContainer.childCount;
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
	}

	public void updateFamilyLine(){
		if (tier == 2) {
			familyLine.enabled = false;
			return;
		}
		familyLine.enabled = true;

		RectTransform rect = GetComponent<RectTransform>();	
		if(!rect.parent){
			return;
		}
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
