using System;
using UnityEngine;

public class ItemSource : MonoBehaviour, IMouseOverUI {

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

	public void ShowUI(GameObject parent, float x, float y) {
		
	}

	public void UpdateUI(float x, float y) {
		throw new NotImplementedException();
	}

	public void HideUI() {
		throw new NotImplementedException();
	}
}
