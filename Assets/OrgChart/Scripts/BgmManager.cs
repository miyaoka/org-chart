using UnityEngine;
using System.Collections;

public class BgmManager : MonoBehaviour {

	AudioClip[] acs;
	AudioSource audio;
	// Use this for initialization
	void Start () {
		acs = Resources.LoadAll<AudioClip>("bgm");
		audio = gameObject.AddComponent<AudioSource>();
		play();
	}
	void play(){
		if(acs.Length == 0){
			return;
		}
		audio.Stop();
		AudioClip ac = acs[Random.Range(0,acs.Length)];
		audio.clip = ac;
		
		audio.Play();
		Debug.Log (ac.name);
		Invoke("play", ac.length);
	}
	// Update is called once per frame
	void Update () {
	
	}
}
