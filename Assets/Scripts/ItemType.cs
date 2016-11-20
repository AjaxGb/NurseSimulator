using UnityEngine;

[CreateAssetMenu]
public class ItemType : ScriptableObject {
	public string Name;
	public Sprite Image;

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
