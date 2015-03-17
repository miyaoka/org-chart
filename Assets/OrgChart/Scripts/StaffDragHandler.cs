using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StaffDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	private Vector3 dragStartOffset;
	public GameObject draggedItem;
	
	void Start(){
	}

	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData)
	{
		Canvas canvas = FindInParents<Canvas>(gameObject);
		if(canvas == null){
			return;
		}
		
		GameSounds.auSelect.Play();
		
		//clone item
		draggedItem = Instantiate(gameObject) as GameObject;

		//remove shadow and line
		Component[] shadows = draggedItem.GetComponentsInChildren<Shadow>();
		foreach(Shadow shadow in shadows){
			Destroy(shadow);
		}
		draggedItem.GetComponentInChildren<UILineRenderer> ().enabled = false;
//		Destroy(draggedItem.GetComponentInChildren<UILineRenderer>());

		//set size and alpha
		ContentSizeFitter csf = draggedItem.AddComponent<ContentSizeFitter>();
		csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
		csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

		CanvasGroup dcg = draggedItem.GetComponent<CanvasGroup>();
		dcg.blocksRaycasts = false;
		dcg.alpha = .35F;		

		//add to canvas
		draggedItem.transform.SetParent(canvas.transform);
		draggedItem.transform.SetAsLastSibling();
		
		//keep pos
		dragStartOffset = (Vector3)eventData.position - transform.position;

		//hide original
		GetComponent<CanvasGroup>().alpha = 0.0F;
//		GetComponent<StaffDropHandler>().enabled = false;

		//notify to all
		EventManager.Instance.TriggerEvent (new StaffBeginDragEvent(gameObject));
		
	}

	#endregion

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
		draggedItem.transform.position = Input.mousePosition - dragStartOffset;
	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{
		Destroy(draggedItem);

		//show original
		GetComponent<CanvasGroup>().alpha = 1.0F;
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
