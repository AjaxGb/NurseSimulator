using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {

	public static InventoryManager inst;

	public ItemStack itemPrefab;
	public ItemType[] test;

	private Dictionary<ItemType, ItemStack> items;
	
	void Start() {
		if (inst != null) {
			Debug.LogWarning("Two InventoryManagers are active at once!");
		} else {
			inst = this;
		}
		items = new Dictionary<ItemType, ItemStack>();
	}

	void Update() {
		if (test.Length != 0) {
			if (Input.GetMouseButtonDown(0)) {
				AddItem(test[Random.Range(0, test.Length)]);
			}
			if (Input.GetMouseButtonDown(1)) {
				RemoveItem(test[Random.Range(0, test.Length)]);
			}
		}
	}

	public ItemStack GetItem(ItemType itemType) {
		ItemStack stack = null;
		items.TryGetValue(itemType, out stack);
		return stack;
	}

	public ItemStack AddItem(ItemType itemType, int count = 1) {
		if (count < 0) {
			throw new System.ArgumentOutOfRangeException("count", "Cannot be negative");
		}
		ItemStack stack = null;
		if (items.TryGetValue(itemType, out stack)) {
			// Already exists
			stack.Count += count;
		} else if (count > 0) {
			stack = Instantiate(itemPrefab).SetItem(itemType, count);
			stack.transform.SetParent(this.transform);
			items.Add(itemType, stack);
		}
		return stack;
	}

	public ItemStack RemoveItem(ItemType itemType, int count = 1) {
		if (count < 0) {
			throw new System.ArgumentOutOfRangeException("count", "Cannot be negative");
		}
		ItemStack stack;
		if (items.TryGetValue(itemType, out stack)) {
			// Already exists
			if (stack.Count < count) {
				throw new System.ArgumentOutOfRangeException("count", "Cannot remove more items than exist");
			}
			stack.Count -= count;
			if (stack.Count == 0) {
				items.Remove(stack.ItemType);
				Destroy(stack.gameObject);
			}
		} else {
			throw new KeyNotFoundException(string.Format("\"{0}\" not found in inventory", itemType));
		}
		return stack;
	}
}