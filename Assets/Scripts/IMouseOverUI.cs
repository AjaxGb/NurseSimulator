using UnityEngine;

public interface IMouseOverUI {
	void ShowUI(Transform parent, Camera camera);
	void UpdateUI(Camera camera);
	void HideUI();
}
