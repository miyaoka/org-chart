using UnityEngine;
using System.Collections;
using System.Reflection;

public class HairSprites : MonoBehaviour {

	[SerializeField] public Sprite hair2;
  [SerializeField] public Sprite hair3;
  [SerializeField] public Sprite hair4;
  [SerializeField] public Sprite hair5;
  [SerializeField] public Sprite hair6;
	
  public Sprite hairByAge(int age){
    FieldInfo f = this.GetType().GetField(
      "hair" + Mathf.Max(2, Mathf.Min (6, Mathf.FloorToInt((float)age / 10)) )
    );
    return f.GetValue(this) as Sprite;
	}	
}
