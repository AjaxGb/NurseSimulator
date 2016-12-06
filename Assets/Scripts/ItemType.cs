using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class ItemType : ScriptableObject {
	public string Name;
	public Sprite Image;
	
	private static Dictionary<string, ItemType> nameMap = new Dictionary<string, ItemType>();

	public void OnEnable() {
		nameMap.Add(this.Name, this);
	}

	public static ItemType FromName(string name) {
		ItemType type;
		nameMap.TryGetValue(name, out type);
		return type;
	}

	public override bool Equals(object obj) {
		if (obj == null || GetType() != obj.GetType()) {
			return false;
		}
		ItemType that = (ItemType)obj;
		return this.Name == that.Name && this.Image == that.Image;
	}

	public override int GetHashCode() {
		return Name != null ? Name.GetHashCode() : 0;
	}

	public override string ToString() {
		return string.Format("{0}#{1}", Name, Image.GetInstanceID());
	}
}
