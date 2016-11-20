using UnityEngine;

public class ItemSource : MonoBehaviour {

	public ItemType itemType;
	public bool infiniteSource;
	public int amountLeft;
	
	void OnMouseDown() {
		if (itemType != null && InventoryManager.inst != null && (infiniteSource || amountLeft > 0)) {
			InventoryManager.inst.AddItem(itemType);
			if (!infiniteSource) {
				amountLeft--;
			}
		}
	}
}
