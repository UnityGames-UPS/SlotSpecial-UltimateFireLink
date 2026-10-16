using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Sits on each Spin button (PC + mobile) and forwards the button's pointer events to SlotBehaviour,
// which owns the hold timer and spin logic: a quick tap runs one spin, holding long enough starts
// auto-spin. No EventTrigger needed — the button already routes pointer events to this component.
public class AutoSpinManager : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler
{
	[SerializeField]
	private SlotBehaviour slotManager;
	[SerializeField]
	private Button StartBTN;


	public void OnPointerDown(PointerEventData eventData)
	{
		slotManager.OnSpinPointerDown(StartBTN);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		slotManager.OnSpinPointerUp(StartBTN);
	}

	// Dragging off the button cancels the pending hold so a release elsewhere can't start auto-spin.
	public void OnPointerExit(PointerEventData eventData)
	{
		slotManager.OnSpinPointerExit();
	}
}
