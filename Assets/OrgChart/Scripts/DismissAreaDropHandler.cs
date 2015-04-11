using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DismissAreaDropHandler : NodeDropHandler, IDropHandler{

  new void Awake(){
    base.Awake ();
  }

  #region IDropHandler implementation
  public void OnDrop (PointerEventData eventData)
  {
    StaffNodePresenter pointerNode = getPointerStaffNode (eventData);
    if (!pointerNode) {
      return;
    }
    GameController.Instance.moveStaffNode (pointerNode, null);
    GameSounds.retire.Play ();
  }
  #endregion
}
