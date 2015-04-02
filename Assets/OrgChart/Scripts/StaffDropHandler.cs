using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StaffDropHandler : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
	[SerializeField] bool isRoot = false;
  private Outline outline;
  private Transform childContainer;
  private StaffNodePresenter thisNode;

  void Awake(){
    outline = GetComponent<Outline> ();
    childContainer = GetComponentInParent<NodePresenter> ().childNodes;
    thisNode = GetComponentInParent<StaffNodePresenter> ();
  }
	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
    StaffNodePresenter pointerNode = eventData.pointerDrag.GetComponentInParent<StaffNodePresenter> ();
    if (thisNode && !thisNode.isAssigned.Value) {
      thisNode.staffId.Value = pointerNode.staffId.Value;
      thisNode.moved = false;
    } 
    else {
      if(pointerNode.transform.parent == childContainer){
        return;
      }
      GameObject newNodeObj = GameController.Instance.createStaffNode ();
      StaffNodePresenter newNode = newNodeObj.GetComponent<StaffNodePresenter> ();
      newNode.staffId.Value = pointerNode.staffId.Value;

      newNodeObj.transform.SetParent (childContainer);
    }


    GameSounds.auDrop.Play();
    pointerNode.moved = true;
	}
	#endregion

	#region IPointerEnterHandler implementation

	public void OnPointerEnter (PointerEventData eventData)
	{
		if (isRoot && !eventData.pointerDrag) {
			return;
		}
    outline.enabled = true;
	}

	#endregion

	#region IPointerExitHandler implementation

	public void OnPointerExit (PointerEventData eventData)
	{
    outline.enabled = false;
	}

	#endregion
}
