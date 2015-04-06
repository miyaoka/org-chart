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

    Canvas canvas = FindInParents<Canvas>(gameObject);
    if(canvas == null){
      return;
    }

    node.isDragging.Value = true;
    GameSounds.auSelect.Play ();


    //clone item
    dragPointer = GameController.Instance.createStaffNode ();
    StaffNodePresenter dragNode = dragPointer.GetComponent<StaffNodePresenter> ();
    dragNode.staffId.Value = node.staffId.Value;

    GameController.Instance.staffRxDataList [node.staffId.Value.Value].age.Value++;
    GameController.Instance.rxint.Value++;

    //set size
    dragPointer.GetComponent<ContentSizeFitter>().enabled = true;



    //remove children
    /*
    StaffPresenter sp = dragPointer.GetComponent<StaffPresenter>();
    sp.removeChildren();
    sp.tier.Value = 1;
    Debug.Log ( sp.shirtsColor.Value);
*/    

    /*
    //remove shadow and line
    Component[] shadows = dragPointer.GetComponentsInChildren<Shadow>();
    foreach(Shadow shadow in shadows){
      Destroy(shadow);
    }
    dragPointer.GetComponentInChildren<UILineRenderer> ().enabled = false;
    //    Destroy(draggedItem.GetComponentInChildren<UILineRenderer>());

    //set size and alpha
    csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
    csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    */


    CanvasGroup dcg = dragPointer.GetComponent<CanvasGroup>();
    dcg.blocksRaycasts = false;
    dcg.alpha = .75F; 

    //add to canvas
    dragPointer.transform.SetParent(canvas.transform);
    dragPointer.transform.SetAsLastSibling();

    //keep pos
//    dragStartOffset = (Vector3)eventData.position - transform.position;

    //hide original
    //    GetComponent<CanvasGroup>().alpha = 0.0F;
    GetComponentInParent<StaffNodePresenter>().isAssigned.Value = false;

//    GetComponent<StaffPresenter>().isAssigned.Value = false;
    //    GetComponent<StaffDropHandler>().enabled = false;

    //notify to all
//    EventManager.Instance.TriggerEvent (new StaffBeginDragEvent(gameObject));

  }

  #endregion

  #region IDragHandler implementation

  public void OnDrag (PointerEventData eventData)
  {
    RectTransform rect = dragPointer.transform as RectTransform;
    rect.position = Input.mousePosition -  new Vector3(4, -4, 0);
  }

  #endregion

  #region IEndDragHandler implementation

  public void OnEndDrag (PointerEventData eventData)
  {

    Destroy(dragPointer);

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

  static public T FindInParents<T>(GameObject go) where T : Component
  {
    if (go == null) return null;
    var comp = go.GetComponent<T>();

    if (comp != null)
      return comp;

    Transform t = go.transform.parent;
    while (t != null && comp == null)
    {
      comp = t.gameObject.GetComponent<T>();
      t = t.parent;
    }
    return comp;
  }

}
