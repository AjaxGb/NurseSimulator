using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IDictionary<ItemType, int> {

	public ItemStack itemPrefab;

	private Dictionary<ItemType, ItemStack> items = new Dictionary<ItemType, ItemStack>();

	public int Count { get { return items.Count; } }
	public int TotalCount { get; private set; }

	public bool IsReadOnly { get { return false; } }
	public ICollection<ItemType> Keys { get { return items.Keys; } }
	public ICollection<int> Values { get { return (ICollection<int>)(from i in items.Values select i.Count); } }
	
	public int this[ItemType key] {
		get {
			ItemStack stack;
			if (items.TryGetValue(key, out stack)) {
				return stack.Count;
			} else {
				return default(int);
			}
		}
		set {
			if (value < 0) {
				throw new ArgumentOutOfRangeException("Cannot be negative");
			} else if (value == 0) {
				items.Remove(key);
			}
			ItemStack stack;
			if (items.TryGetValue(key, out stack)) {
				TotalCount -= stack.Count;
				stack.Count = value;
			} else {
				stack = Instantiate(itemPrefab).SetItem(key, value);
				stack.transform.SetParent(this.transform);
				items.Add(key, stack);
			}
			TotalCount += stack.Count;
		}
	}

	public bool Contains(KeyValuePair<ItemType, int> item) {
		ItemStack stack;
		return items.TryGetValue(item.Key, out stack) && stack.Count == item.Value;
	}

	public bool ContainsKey(ItemType key) {
		return items.ContainsKey(key);
	}

	public bool TryGetValue(ItemType key, out int value) {
		ItemStack stack;
		if (items.TryGetValue(key, out stack)) {
			value = stack.Count;
			return true;
		} else {
			value = default(int);
			return false;
		}
	}

	public void Clear() {
		foreach (var item in items.Values) {
			Destroy(item.gameObject);
		}
		items.Clear();
		TotalCount = 0;
	}

	public IEnumerable<KeyValuePair<ItemType, int>> AsEnumerable() {
		return from i in items select new KeyValuePair<ItemType, int>(i.Key, i.Value.Count);
	}

	IEnumerator<KeyValuePair<ItemType, int>> IEnumerable<KeyValuePair<ItemType, int>>.GetEnumerator() {
		return AsEnumerable().GetEnumerator();
	}

	public IEnumerator GetEnumerator() {
		return AsEnumerable().GetEnumerator();
	}

	public void CopyTo(KeyValuePair<ItemType, int>[] array, int arrayIndex) {
		if (array.Length - arrayIndex < Count) throw new ArgumentException("Not enough space", "array");
		foreach (var kvp in AsEnumerable()) {
			array[arrayIndex] = kvp;
			arrayIndex++;
		}
	}

	public void Add(KeyValuePair<ItemType, int> item) {
		Add(item.Key, item.Value);
	}

	public void Add(ItemType itemType, int count = 1) {
		if (count < 0) {
			throw new ArgumentOutOfRangeException("count", "Cannot be negative");
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
		TotalCount += count;
	}

	/// <summary>
	/// Removes as many items as possible
	/// </summary>
	/// <param name="itemType">Type of item to remove</param>
	/// <param name="count">Max number of items to remove</param>
	/// <returns>Actual number of items removed</returns>
	public int RemoveCount(ItemType itemType, int count = 1) {
		if (count < 0) {
			throw new ArgumentOutOfRangeException("count", "Cannot be negative");
		}
		ItemStack stack;
		if (items.TryGetValue(itemType, out stack)) {
			if (stack.Count < count) {
				count = stack.Count;
			}
			stack.Count -= count;
			if (stack.Count == 0) {
				items.Remove(stack.ItemType);
				Destroy(stack.gameObject);
			}
		} else {
			count = 0;
		}
		TotalCount -= count;
		return count;
	}

	/// <summary>
	/// Removes the specified amount of items if possible, none otherwise
	/// </summary>
	/// <param name="item">Pair specifying item type and count</param>
	/// <returns>Whether the items were removed</returns>
	public bool Remove(KeyValuePair<ItemType, int> item) {
		ItemStack stack;
		if (items.TryGetValue(item.Key, out stack) && stack.Count >= item.Value) {
			TotalCount -= item.Value;
			stack.Count -= item.Value;
			if (stack.Count == 0) {
				items.Remove(stack.ItemType);
				Destroy(stack.gameObject);
			}
			return true;
		} else {
			return false;
		}
	}

	/// <summary>
	/// Removes all items of type
	/// </summary>
	/// <param name="itemType">Type of items to remove</param>
	/// <returns>Whether there were any item to remove</returns>
	public bool Remove(ItemType itemType) {
		ItemStack stack;
		if (items.TryGetValue(itemType, out stack)) {
			TotalCount -= stack.Count;
			items.Remove(stack.ItemType);
			Destroy(stack.gameObject);
			return true;
		} else {
			return false;
		}
	}
}