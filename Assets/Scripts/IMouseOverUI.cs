using UnityEngine;
using System.Collections;

public interface IMouseOverUI {
	void ShowUI(GameObject parent, float x, float y);
	void UpdateUI(float x, float y);
	void HideUI();
}
