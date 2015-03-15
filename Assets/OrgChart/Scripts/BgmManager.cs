using UnityEngine;
using System.Collections;

public class BgmManager : MonoBehaviour {

	AudioClip[] acs;
	AudioSource au;
	// Use this for initialization
	void Start () {
		acs = Resources.LoadAll<AudioClip>("bgm");
		au = gameObject.AddComponent<AudioSource>();
		play();
	}
	void play(){
		if(acs.Length == 0){
			return;
		}
		au.Stop();
		AudioClip ac = acs[Random.Range(0,acs.Length)];
		au.clip = ac;
		
		au.Play();
		Debug.Log (ac.name);
		Invoke("play", ac.length);
	}
	// Update is called once per frame
	void Update () {
	
	}
}
