using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;

public class StaffNodeDropHandler : NodeDropHandler, IDropHandler {
  protected StaffNodePresenter staffNode;

  new void Awake(){
    base.Awake ();
    staffNode = GetComponentInParent<StaffNodePresenter> ();
  }
	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
    StaffNodePresenter pointerNode = getPointerStaffNode (eventData);
    if (!pointerNode || pointerNode == staffNode) {
      return;
    }
    GameController.Instance.moveStaffNode (pointerNode, staffNode);
	}
	#endregion

	#region IPointerEnterHandler implementation

  public override void OnPointerEnter (PointerEventData eventData)
	{
    if (staffNode.isAssigned.Value || getPointerStaffNode(eventData) ) {
      outline.enabled = true;
		}
	}

	#endregion
}
