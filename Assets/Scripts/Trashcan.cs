using System;
using UnityEngine;

public class Trashcan : MonoBehaviour, IMouseOverUI {
	public void ShowUI(Transform parent, Camera camera) {}
	public void UpdateUI(Camera camera, Vector3 point) {}
	public void HideUI() {}

	public void OnClick(int button = 0) {
		Player.inst.inventory.Clear();
	}
}
