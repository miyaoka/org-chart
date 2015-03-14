using UnityEngine;
using System.Collections;

public class GameSounds : MonoBehaviour {

	public static AudioSource auSelect;
	public static AudioSource auDrop;
	    
	// Use this for initialization
	void Start () {
		AudioSource[] audios = GetComponents<AudioSource>();
		auSelect = audios[0];
		auDrop = audios[1];
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
