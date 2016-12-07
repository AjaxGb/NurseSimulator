using UnityEngine;

public class ItemSource : MonoBehaviour, IMouseOverUI {

	public ItemType itemType;
	public bool infiniteSource;
	public int amountLeft;
	public ItemStack popupPrefab;

	private ItemStack currPopup;
	
	public void OnClick(int button = 0) {
		if (itemType != null && Player.inst.inventory != null && (infiniteSource || amountLeft > 0)) {
			if (button == 0) {
				Player.inst.inventory.AddCount(itemType, 1);
			} else {
				Player.inst.inventory.RemoveCount(itemType, 1);
			}
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
