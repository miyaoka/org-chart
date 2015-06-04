using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StaffNodeDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

  private GameObject dragPointer;
  private StaffNodePresenter node;

  void Start(){
    node = GetComponentInParent<StaffNodePresenter> ();
  }

  #region IBeginDragHandler implementation

  public void OnBeginDrag (PointerEventData eventData)
  {
    GameSounds.select.Play ();

    //clone item
    dragPointer = GameController.Instance.createStaffCursor(node.staffData);

    GameController.Instance.draggingNode.Value = node;

    //begin drag
    node.isDragging.Value = true;

    //hide original
    node.isAssigned.Value = false;

    //notify to all
//    EventManager.Instance.TriggerEvent (new StaffBeginDragEvent(gameObject));

  }

  #endregion

  #region IDragHandler implementation

  public void OnDrag (PointerEventData eventData)
  {
    RectTransform rect = dragPointer.transform as RectTransform;
    rect.position = Input.mousePosition +  new Vector3(0, 40, 0);
  }

  #endregion

  #region IEndDragHandler implementation

  public void OnEndDrag (PointerEventData eventData)
  {
    GameController.Instance.draggingNode.Value = null;

    Destroy (dragPointer);

//    EventManager.Instance.TriggerEvent (new StaffEndDragEvent ());

    if (node.isMoved) {
      node.isMoved = false;
      enabled = false;
    } else {
      node.isAssigned.Value = true;
    }
    node.isDragging.Value = false;

  }

  #endregion


}
