using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NodeDropHandler :MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
  private Outline outline;

  protected void Awake(){
    outline = GetComponent<Outline> ();
  }
  void Start(){
  }
  protected StaffNodePresenter getPointerStaffNode(PointerEventData eventData){
    return eventData.pointerDrag ? eventData.pointerDrag.GetComponentInParent<StaffNodePresenter> () : null;
  }

  #region IPointerEnterHandler implementation

  public virtual void OnPointerEnter (PointerEventData eventData)
  {
    if (getPointerStaffNode(eventData)) {
      outline.enabled = true;
    }
  }

  #endregion

  #region IPointerExitHandler implementation

  public virtual void OnPointerExit (PointerEventData eventData)
  {
    outline.enabled = false;
  }

  #endregion
}


