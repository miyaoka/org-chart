using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	[SerializeField] RectTransform recruitContainer;
	[SerializeField] RectTransform staffContainer;
	[SerializeField] GameObject staffPrefab;

	private float shirtsV = .9F;
	private float tieV = .6F;
	private float suitsV = .6F;

	// Use this for initialization
	void Start () {
		SetupListeners();
        foreach( Transform child in staffContainer){
			Destroy(child.gameObject);
        }
        
		updateRecruits ();
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
		StaffData sd = data;
		data = sd;
		
//		StartCoroutine (updateFamilyTreeOnNextFrame() );
//        updateInfo();
    }
	public StaffData data{
		get{
			StaffData sd = new StaffData();
			
			sd.children = new StaffData[staffContainer.childCount];
			for(int i = 0; i < staffContainer.childCount; i++){	
				Transform child = staffContainer.GetChild(i);
                sd.children[i] = child.GetComponent<StaffController>().data;
			}
			
			return sd;
			
		}
		set{
            foreach(Transform child in staffContainer){
            	Destroy(child.gameObject);
            }
			for(int i = 0; i < value.children.Length; i++){
				addChild(value.children[i], staffContainer, 0);
            }
        }
    }

    void addChild(StaffData sd, Transform parent, int tier){
		GameObject child = Instantiate(staffPrefab) as GameObject;
		StaffController sc = child.GetComponent<StaffController>();
		sd.tier = tier;
		sc.data = sd;
        child.transform.SetParent(parent);
        tier++;
		for(int i = 0; i < sd.children.Length; i++){
			addChild(sd.children[i], sc.childrenContainer, tier);
		}
		sc.updateInfo();
    }
    
    
    void updateRecruits(){
		foreach( Transform child in recruitContainer){
			Destroy(child.gameObject);
		}
		int count = Random.Range (25, 25);
		for(int i = 0; i < count; i++){
			createStaff().transform.SetParent(recruitContainer);
		}

	}
	
	GameObject createStaff(){
		GameObject staff = Instantiate(staffPrefab) as GameObject;
		StaffController sc = staff.GetComponent<StaffController>();
		int age = Random.Range(0,45);
		float baseSkill = 0;
		for(int i = 0; i < age; i++){
			if(.5F <= Random.value){
				baseSkill++;
			}
		}
		StaffData sd = new StaffData();
		sd.baseSkill = Mathf.FloorToInt(baseSkill * .8F);
		sd.age = age + 20;

		float shirtsHue = Random.value;
		float tieHue = (.5F > Random.value) ? nearHue(shirtsHue) : compHue(shirtsHue);
		float suitsHue = (.5F > Random.value) ? nearHue(shirtsHue) : compHue(shirtsHue);

		sd.shirtsColor = HSVToRGB (shirtsHue, Random.value * .2F, shirtsV);
		sd.tieColor = HSVToRGB (tieHue, Random.value * .2F + .2F, tieV);
		sd.suitsColor = HSVToRGB (suitsHue, Random.value * .3F, suitsV);
		
		sc.data = sd;
		
		
		return staff;
	
	}
	float nearHue(float hue){
		return (hue + Random.value * 1F/6F - 1F/12F + 1F ) % 1F;
	}
	float compHue(float hue){
		return (hue + .5F) % 1F;
	}
	public static Color HSVToRGB(float H, float S, float V)
	{
		if (S == 0f) {
			return new Color (V, V, V);
		}
		else if (V == 0f){
			return Color.black;
		}
		else
		{
			Color col = Color.black;
			float Hval = H * 6f;
			int sel = Mathf.FloorToInt(Hval);
			float mod = Hval - sel;
			float v1 = V * (1f - S);
			float v2 = V * (1f - S * mod);
			float v3 = V * (1f - S * (1f - mod));
			switch (sel + 1)
			{
			case 0:
				col.r = V;
				col.g = v1;
				col.b = v2;
				break;
			case 1:
				col.r = V;
				col.g = v3;
				col.b = v1;
				break;
			case 2:
				col.r = v2;
				col.g = V;
				col.b = v1;
				break;
			case 3:
				col.r = v1;
				col.g = V;
				col.b = v3;
				break;
			case 4:
				col.r = v1;
				col.g = v2;
				col.b = V;
				break;
			case 5:
				col.r = v3;
				col.g = v1;
				col.b = V;
				break;
			case 6:
				col.r = V;
				col.g = v1;
				col.b = v2;
				break;
			case 7:
				col.r = V;
				col.g = v3;
				col.b = v1;
				break;
			}
			col.r = Mathf.Clamp(col.r, 0f, 1f);
			col.g = Mathf.Clamp(col.g, 0f, 1f);
			col.b = Mathf.Clamp(col.b, 0f, 1f);
			return col;
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
