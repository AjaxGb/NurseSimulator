using UnityEngine;

public class ItemSource : MonoBehaviour, IMouseOverUI {

	public ItemType itemType;
	public bool infiniteSource;
	public int amountLeft;
	public RectTransform popupPrefab;

	private RectTransform currPopup;
	
	void OnMouseDown() {
		if (itemType != null && InventoryManager.inst != null && (infiniteSource || amountLeft > 0)) {
			InventoryManager.inst.AddItem(itemType);
			if (!infiniteSource) {
				amountLeft--;
			}
		}
	}

	public void ShowUI(Transform parent, Camera camera) {
		if (currPopup != null) {
			HideUI();
		}
		if (popupPrefab != null) {
			currPopup = Instantiate(popupPrefab);
			currPopup.SetParent(parent);
		}
	}

	public void UpdateUI(Camera camera) {
		if (currPopup != null) {
			currPopup.position = RectTransformUtility.WorldToScreenPoint(camera, this.transform.position);
		}
	}

	public void HideUI() {
		if (currPopup != null) {
			Destroy(currPopup.gameObject);
		}
	}
}
