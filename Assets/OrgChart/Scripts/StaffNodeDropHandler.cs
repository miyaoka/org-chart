using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StaffNodeDropHandler : NodeDropHandler {
  protected StaffNodePresenter thisNode;

  void Awake(){
    base.Awake ();
    thisNode = GetComponentInParent<StaffNodePresenter> ();
  }
	#region IDropHandler implementation
	public override void OnDrop (PointerEventData eventData)
	{
    StaffNodePresenter pointerNode = getPointerStaffNode (eventData);
    if (!pointerNode) {
      return;
    }
    if (thisNode && !thisNode.isAssigned.Value) {
      thisNode.staffId.Value = pointerNode.staffId.Value;
      thisNode.isMoved = false;
    } 
    else {
      if(pointerNode.transform.parent == childContainer){
        return;
      }
      GameObject newNodeObj = GameController.Instance.createStaffNode ();
      StaffNodePresenter newNode = newNodeObj.GetComponent<StaffNodePresenter> ();
      newNode.staffId.Value = pointerNode.staffId.Value;
      newNode.isHired = true;

      newNodeObj.transform.SetParent (childContainer);
    }


    GameSounds.auDrop.Play();
    pointerNode.isMoved = true;
	}
	#endregion

	#region IPointerEnterHandler implementation

  public override void OnPointerEnter (PointerEventData eventData)
	{
    if (thisNode.isAssigned.Value || getPointerStaffNode(eventData) ) {
      outline.enabled = true;
		}
	}

	#endregion

}
