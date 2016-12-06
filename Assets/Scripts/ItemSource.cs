using UnityEngine;

public class ItemSource : MonoBehaviour, IMouseOverUI {

	public ItemType itemType;
	public bool infiniteSource;
	public int amountLeft;
	public ItemStack popupPrefab;

	private ItemStack currPopup;
	
	void OnMouseDown() {
		Debug.Log("Clicked!");
		if (itemType != null && InventoryManager.inst != null && (infiniteSource || amountLeft > 0)) {
			InventoryManager.inst.AddItem(itemType);
			if (!infiniteSource) {
				amountLeft--;
				if (currPopup != null) currPopup.Count = amountLeft;
			} else {
				if (currPopup != null) currPopup.Count = -1;
			}
		}
	}

	public void ShowUI(Transform parent, Camera camera) {
		if (currPopup != null) {
			HideUI();
		}
		if (popupPrefab != null) {
			currPopup = Instantiate(popupPrefab);
			currPopup.transform.SetParent(parent);
			currPopup.SetItem(itemType, infiniteSource ? -1 : amountLeft);
		}
	}

	public void UpdateUI(Camera camera, Vector3 point) {
		if (currPopup != null) {
			currPopup.transform.position = RectTransformUtility.WorldToScreenPoint(camera, point);
		}
	}

	public void HideUI() {
		if (currPopup != null) {
			Destroy(currPopup.gameObject);
		}
	}
}
