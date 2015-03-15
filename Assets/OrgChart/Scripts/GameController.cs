using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	[SerializeField] RectTransform recruitContainer;
	[SerializeField] RectTransform staffContainer;
	[SerializeField] GameObject staffPrefab;
	
	// Use this for initialization
	void Start () {
		foreach( Transform child in staffContainer){
			Destroy(child.gameObject);
        }
		foreach( Transform child in recruitContainer){
			Destroy(child.gameObject);
        }
        
        for(int i = 0; i < 5; i++){
        	createStaff().transform.SetParent(recruitContainer);
        }
	}
	
	GameObject createStaff(){
		GameObject staff = Instantiate(staffPrefab) as GameObject;
		StaffController sc = staff.GetComponent<StaffController>();
		int age = Random.Range(0,40);
		float baseSkill = 0;
		for(int i = 0; i < age; i++){
			if(.5F <= Random.value){
				baseSkill++;
			}
		}
		StaffData sd = new StaffData();
		sd.baseSkill = Mathf.FloorToInt(baseSkill * .8F);
		sd.age = age + 20;
		sc.setData(sd);
		
		return staff;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
