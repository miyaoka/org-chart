using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StaffDropHandler : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
	[SerializeField] Transform chindrenContainer;
	[SerializeField] bool isRoot = false;
	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
	/*
		Debug.Log("onDrop");
		Debug.Log(chindrenContainer.position);
		Debug.Log(eventData.pointerDrag.transform.position);
*/

		GameObject draggedItem = eventData.pointerDrag;
		if(draggedItem.transform.parent == chindrenContainer){
			return;
		}
//		Debug.Log (draggedItem.transform.parent.position);
//		Debug.Log (chindrenContainer.position);
		
		GameSounds.auDrop.Play();
//		draggedItem.GetComponent<StaffController>().auDrop.Play();
		draggedItem.transform.SetParent(chindrenContainer, true);
//		eventData.pointerDrag.GetComponent<StaffController>().update();
		
		
//		(eventData.pointerDrag.transform as RectTransform).position.x = 0;
//		Debug.Log ( (eventData.pointerDrag.transform.parent.transform as RectTransform).position);
//		Debug.Log ( (eventData.pointerDrag.transform as RectTransform).position);
	}
	#endregion

	#region IPointerEnterHandler implementation

	public void OnPointerEnter (PointerEventData eventData)
	{
		GetComponent<Outline>().enabled = true;
	}

	#endregion

	#region IPointerExitHandler implementation

	public void OnPointerExit (PointerEventData eventData)
	{
		GetComponent<Outline>().enabled = false;
	}

	#endregion
}
