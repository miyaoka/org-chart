using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;

public class StaffNodeDropHandler : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler{
  protected StaffNodePresenter staffNode;
  [SerializeField] Outline outline;
  [SerializeField] RectTransform panel;

  new void Awake(){
//    base.Awake ();
    staffNode = GetComponentInParent<StaffNodePresenter> ();

//    outline = GetComponentInParent<Outline> ();
//    outline =  <Outline> ();
  }
  protected StaffNodePresenter getPointerStaffNode(PointerEventData eventData){
    return eventData.pointerDrag ? eventData.pointerDrag.GetComponentInParent<StaffNodePresenter> () : null;
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

  public void OnPointerEnter (PointerEventData eventData)
	{
    if (staffNode.isEmpty.Value || getPointerStaffNode(eventData) ) {
      panel.localScale = new Vector3 (1.5f, 1.5f, 1f);
      outline.enabled = true;
		}
	}

	#endregion
  public void OnPointerExit (PointerEventData eventData)
  {
    panel.localScale = new Vector3 (1f, 1f, 1f);
    outline.enabled = false;
  }
}
