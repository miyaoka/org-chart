using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StaffDropHandler : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
	[SerializeField] Transform chindrenContainer;
	[SerializeField] Outline outLine;
	[SerializeField] bool isRoot = false;
	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
		GameObject draggedItem = eventData.pointerDrag;
		if(draggedItem.transform.parent == chindrenContainer){
			return;
		}

		GameSounds.auDrop.Play();
		draggedItem.transform.SetParent(chindrenContainer, true);

		EventManager.Instance.TriggerEvent (new ChartChangeEvent() );
		

	}
	#endregion

	#region IPointerEnterHandler implementation

	public void OnPointerEnter (PointerEventData eventData)
	{
		if (isRoot && !eventData.pointerDrag) {
			return;
		}
		outLine.enabled = true;
	}

	#endregion

	#region IPointerExitHandler implementation

	public void OnPointerExit (PointerEventData eventData)
	{
		outLine.enabled = false;
	}

	#endregion
}
