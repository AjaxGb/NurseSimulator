using UnityEngine;

public interface IMouseOverUI {
	void ShowUI(Transform parent, Camera camera);
	void UpdateUI(Camera camera, Vector3 point);
	void HideUI();
	void OnClick(int button = 0);
}
