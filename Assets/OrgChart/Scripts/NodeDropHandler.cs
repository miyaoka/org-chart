using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NodeDropHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler{
  protected Outline outline;
  protected Transform childContainer;

  protected void Awake(){
    outline = GetComponent<Outline> ();
    childContainer = GetComponentInParent<NodePresenter> ().childNodes;
  }
  void Start(){
  }
  protected StaffNodePresenter getPointerStaffNode(PointerEventData eventData){
    return eventData.pointerDrag ? eventData.pointerDrag.GetComponentInParent<StaffNodePresenter> () : null;
  }
  #region IDropHandler implementation
  public virtual void OnDrop (PointerEventData eventData)
  {
    StaffNodePresenter pointerNode = getPointerStaffNode (eventData);
    if (!pointerNode) {
      return;
    }
    GameObject newNodeObj = GameController.Instance.createStaffNode ();
    StaffNodePresenter newNode = newNodeObj.GetComponent<StaffNodePresenter> ();
    newNode.staffId.Value = pointerNode.staffId.Value;
    newNode.isHired = true;

    newNodeObj.transform.SetParent (childContainer);


    GameSounds.auDrop.Play();
    pointerNode.isMoved = true;
  }
  #endregion

  #region IPointerEnterHandler implementation

  public virtual void OnPointerEnter (PointerEventData eventData)
  {
    if (getPointerStaffNode(eventData)) {
      outline.enabled = true;
    }
  }

  #endregion

  #region IPointerExitHandler implementation

  public void OnPointerExit (PointerEventData eventData)
  {
    outline.enabled = false;
  }

  #endregion
}
