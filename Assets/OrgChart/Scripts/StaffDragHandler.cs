using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StaffDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

  private Vector3 dragStartOffset;
  public GameObject dragPointer;

  void Start(){
  }

  #region IBeginDragHandler implementation

  public void OnBeginDrag (PointerEventData eventData)
  {
    Canvas canvas = FindInParents<Canvas>(gameObject);
    if(canvas == null){
      return;
    }

    GameSounds.auSelect.Play ();

    //clone item
    //    dragPointer = Instantiate(gameObject) as GameObject;
    dragPointer = GetComponent<StaffPresenter>().clone();

    //remove children
    /*
    StaffPresenter sp = dragPointer.GetComponent<StaffPresenter>();
    sp.removeChildren();
    sp.tier.Value = 1;
    Debug.Log ( sp.shirtsColor.Value);
*/    

    //remove shadow and line
    Component[] shadows = dragPointer.GetComponentsInChildren<Shadow>();
    foreach(Shadow shadow in shadows){
      Destroy(shadow);
    }
    dragPointer.GetComponentInChildren<UILineRenderer> ().enabled = false;
    //    Destroy(draggedItem.GetComponentInChildren<UILineRenderer>());

    //set size and alpha
    ContentSizeFitter csf = dragPointer.AddComponent<ContentSizeFitter>();
    csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
    csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

    CanvasGroup dcg = dragPointer.GetComponent<CanvasGroup>();
    dcg.blocksRaycasts = false;
    dcg.alpha = .75F;   

    //add to canvas
    dragPointer.transform.SetParent(canvas.transform);
    dragPointer.transform.SetAsLastSibling();

    //keep pos
    dragStartOffset = (Vector3)eventData.position - transform.position;

    //hide original
    //    GetComponent<CanvasGroup>().alpha = 0.0F;

    GetComponent<StaffPresenter>().isAssigned.Value = false;
    //    GetComponent<StaffDropHandler>().enabled = false;

    //notify to all
    EventManager.Instance.TriggerEvent (new StaffBeginDragEvent(gameObject));

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

    //show original
    //    GetComponent<CanvasGroup>().alpha = 1.0F;
    GetComponent<StaffPresenter>().isAssigned.Value = true;
    //    GetComponent<StaffDropHandler>().enabled = true;

    EventManager.Instance.TriggerEvent (new StaffEndDragEvent ());

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
