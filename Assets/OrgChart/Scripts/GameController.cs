using UnityEngine;
using System.Collections;

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
        
        for(int i = 0; i < 10; i++){
        	createStaff().transform.SetParent(recruitContainer);
        }
	}
	
	GameObject createStaff(){
		GameObject staff = Instantiate(staffPrefab) as GameObject;
		return staff;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
