using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		EventManager.Instance.AddListener<StaffBeginDragEvent> (onStaffBeginDrag);
		EventManager.Instance.AddListener<StaffEndDragEvent> (onStaffEndDrag);

	}
	void onStaffBeginDrag(StaffBeginDragEvent e){
		gameObject.SetActive (false);
	}
	void onStaffEndDrag(StaffEndDragEvent e){
		gameObject.SetActive (true);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
