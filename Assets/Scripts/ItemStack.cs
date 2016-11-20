using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemStack : MonoBehaviour {
	private ItemType _itemType;
	public ItemType ItemType {
		get { return _itemType; }
		private set {
			Start();
			_itemType = value;
			_uiImage.sprite = _itemType.Image;
			_uiName.text = value.Name;
		}
	}

	private int _count;
	public int Count {
		get { return _count; }
		set {
			Start();
			_count = value;

			if (value > 1) {
				_uiMult.text = '\u00D7' + value.ToString();
				_uiMult.enabled = true;
			} else {
				_uiMult.enabled = false;
			}
		}
	}

	private bool initialized = false;
	private Text _uiName, _uiMult;
	private Image _uiImage;
	
	void Start() {
		if (!initialized) {
			_uiImage = GetComponentInChildren<Image>();
			Text[] texts = GetComponentsInChildren<Text>();
			_uiName = texts.First(t => t.name == "Name");
			_uiMult = texts.First(t => t.name == "Multiplier");
		}
	}

	public ItemStack SetItem(ItemType itemType, int count = 1) {
		if (ItemType != null) {
			throw new InvalidOperationException("Item is already set");
		}
		ItemType = itemType;
		Count = count;
		return this;
	}
}
