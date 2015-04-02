using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StaffDropHandler : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
	[SerializeField] bool isRoot = false;
  private Outline outline;
  private NodePresenter np;
  private Transform childContainer;

  void Awake(){
    outline = GetComponent<Outline> ();
    childContainer = GetComponentInParent<NodePresenter> ().childNodes;
  }
	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
    StaffNodePresenter pointerNodePresenter = eventData.pointerDrag.GetComponentInParent<StaffNodePresenter> ();
    if(pointerNodePresenter.transform.parent == childContainer){
      return;
    }

    GameSounds.auDrop.Play();
    GameObject newNode = GameController.Instance.createStaffNode ();
    StaffNodePresenter newNodePresenter = newNode.GetComponent<StaffNodePresenter> ();
    newNodePresenter.staffId.Value = pointerNodePresenter.staffId.Value;

    newNode.transform.SetParent (childContainer);
	}
	#endregion

	#region IPointerEnterHandler implementation

	public void OnPointerEnter (PointerEventData eventData)
	{
		if (isRoot && !eventData.pointerDrag) {
			return;
		}
    outline.enabled = true;
	}

	#endregion

	#region IPointerExitHandler implementation

	public void OnPointerExit (PointerEventData eventData)
	{
    outline.enabled = false;
	}

	#endregion
}
