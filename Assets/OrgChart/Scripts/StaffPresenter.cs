using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;
using Unity.Linq;
using UnityEngine.EventSystems;

public class StaffPresenter : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	//view
	[SerializeField] public RectTransform childrenContainer;
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

	[SerializeField] GameObject profileAssigned;
	[SerializeField] GameObject profileEmpty;
	
	//model
	public ReadOnlyReactiveProperty<int> currentSkill { get; private set; }
	
	public IntReactiveProperty baseSkill =  new IntReactiveProperty();	
	public IntReactiveProperty age = new IntReactiveProperty();
	public IntReactiveProperty tier = new IntReactiveProperty();
	public ReactiveProperty<Color> shirtsColor = new ReactiveProperty<Color>();
	public ReactiveProperty<Color> tieColor = new ReactiveProperty<Color>();
	public ReactiveProperty<Color> suitsColor = new ReactiveProperty<Color>();
	public ReactiveProperty<Color> faceColor = new ReactiveProperty<Color>();
	public ReactiveProperty<Color> hairColor = new ReactiveProperty<Color>();
	public ReactiveProperty<bool> isHired = new ReactiveProperty<bool>();
	public ReactiveProperty<bool> isAssigned = new ReactiveProperty<bool>(true);
	
	IObservable<int> childCountStream;
	IObservable<Vector2> parentDelta;
	IObservable<Vector2> thisDelta;

	CompositeDisposable eventResources = new CompositeDisposable();
	
	public GameObject dragPointer;

	private float familyLineHeight = 19.0F;

	// Use this for initialization
	void Start(){
        

		//define model
		childCountStream = 
			Observable
				.EveryUpdate()
				.Select(_ => childrenContainer ? childrenContainer.childCount : 0)
				.DistinctUntilChanged();
		
		
		currentSkill = 
			baseSkill
				.CombineLatest(childCountStream, (s, c) => s - c )
				.ToReadOnlyReactiveProperty();
		
		//model to view
		currentSkill
			.SubscribeToText(currentSkillText)
			.AddTo(eventResources);
		
		baseSkill
			.CombineLatest(childCountStream, (s, c) => 0 < c ? "/" + s : "" )
				.SubscribeToText(baseSkillText)
				.AddTo(eventResources);
		
		age
			.SubscribeToText(ageText, x => "(" + x.ToString() + ")" )
				.AddTo(eventResources);
		
		age
			.Subscribe(x => hair.sprite = hairSprites.hairByAge(x));
		
		shirtsColor
			.Subscribe(x => shirts.color = x);
		tieColor
			.Subscribe(x => tie.color = x);
		suitsColor
			.Subscribe(x => suits.color = x);
		
		tier
			.CombineLatest(childCountStream, (t, c) => (0 < c) ? Mathf.Min(t, 1) : t)
				.Subscribe(t => {
					if(t == 1){
						tie.enabled = true;
						suits.enabled = true;
					} else if(t == 2){
						tie.enabled = true;
						suits.enabled = false;			
					} else {
						tie.enabled = false;
						suits.enabled = false;						
					}
				})
				.AddTo(eventResources);
		isAssigned
			.Subscribe(x => {
				profileAssigned.SetActive(x);
				profileEmpty.SetActive(!x);
				//hide if not assigned and have no children
				GetComponent<CanvasGroup>().alpha = (!x && childrenContainer.childCount == 0) ? 0 : 1;
			});
		
		thisDelta = 
			Observable
				.EveryUpdate()
				.Select(_ => (this.transform as RectTransform).sizeDelta)
				.DistinctUntilChanged();
		parentDelta = 
			Observable
				.EveryUpdate()
				.Select(_ => transform.parent ? (transform.parent.transform as RectTransform).sizeDelta : new Vector2())
				.DistinctUntilChanged();
		
		thisDelta
			.CombineLatest(parentDelta, (td, pd) => new Vector2(
				td.x/2, 
				pd.x/2 - transform.position.x + (transform.parent ? transform.parent.transform.position.x : 0)
			))
			.Subscribe(x => drawFamilyLine(x))
			.AddTo(eventResources);
		
	}
	void OnDestroy()
	{
		eventResources.Dispose();
	}
	
	// Update is called once per frame
	void Update () {

	}
	void drawFamilyLine(Vector2 pt){
		familyLine.Points = new Vector2[] { 
			new Vector2(pt.x, 0), 
			new Vector2(pt.x, 8),//familyLineHeight * .4F),
			new Vector2(pt.y, 8),//familyLineHeight * .4F),
//			new Vector2(pt.y, familyLineHeight * .6F),			
			new Vector2(pt.y, familyLineHeight)
		};
		familyLine.SetVerticesDirty();		
	}
	public GameObject clone(){
		GameObject go = Instantiate(gameObject);
		StaffPresenter sc = go.GetComponent<StaffPresenter>();
        sc.shirtsColor = shirtsColor;
		sc.tieColor = tieColor;
		sc.suitsColor = suitsColor;
		foreach (Transform child in sc.childrenContainer){
			Destroy(child.gameObject);
        }		
        sc.setPointer(true);
        return go;
	}
	public void setPointer(bool isPointer){
		//hide shadow and line if isPointer
		GetComponentInChildren<Shadow>().enabled = !isPointer;
		GetComponentInChildren<UILineRenderer>().enabled = !isPointer;
		
		//enable size fitter if isPointer
		GetComponent<ContentSizeFitter>().enabled = isPointer;
		
		CanvasGroup dcg = GetComponent<CanvasGroup>();
        dcg.blocksRaycasts = !isPointer;
        dcg.alpha = isPointer ? .5f : 1;
	}


	#region IBeginDragHandler implementation
	
	public void OnBeginDrag (PointerEventData eventData)
	{
		Canvas canvas = FindInParents<Canvas>(gameObject);
		if(canvas == null){
			return;
		}
		
		GameSounds.auSelect.Play();
		
		//create pointer
		dragPointer = clone();
		
		//add to canvas
		dragPointer.transform.SetParent(canvas.transform);
		dragPointer.transform.SetAsLastSibling();
		
		
		//hide original
		GetComponent<StaffPresenter>().isAssigned.Value = false;
		
		//notify to all
		EventManager.Instance.TriggerEvent (new StaffBeginDragEvent(gameObject));
		
	}
	
	#endregion
	
	#region IDragHandler implementation
	
	public void OnDrag (PointerEventData eventData)
	{
		RectTransform rect = dragPointer.transform as RectTransform;
		rect.position = Input.mousePosition -  new Vector3(4, -4, 0);
	}
	
	#endregion
	
	#region IEndDragHandler implementation
	
	public void OnEndDrag (PointerEventData eventData)
	{
		Debug.Log ("end drag");
        Destroy(dragPointer);
		
		//show original
		//		GetComponent<CanvasGroup>().alpha = 1.0F;
		GetComponent<StaffPresenter>().isAssigned.Value = true;
		//		GetComponent<StaffDropHandler>().enabled = true;
		
		EventManager.Instance.TriggerEvent (new StaffEndDragEvent ());
        
    }
    
    #endregion
    
    static public T FindInParents<T>(GameObject go) where T : Component
    {
        if (go == null) return null;
        var comp = go.GetComponent<T>();
        
        if (comp != null)
            return comp;
        
        Transform t = go.transform.parent;
        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }
        return comp;
	}
}
