using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OrgNodeDropHandler : NodeDropHandler, IDropHandler{
  protected NodePresenter orgNode;

  protected void Awake(){
    base.Awake ();
    orgNode = GetComponentInParent<NodePresenter> ();
  }

  #region IDropHandler implementation
  public void OnDrop (PointerEventData eventData)
  {
    StaffNodePresenter pointerNode = getPointerStaffNode (eventData);
    if (!pointerNode) {
      return;
    }
    GameController.Instance.moveStaffNode (pointerNode, orgNode);
  }
  #endregion
}
