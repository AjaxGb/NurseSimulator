using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class CheckAvailable : MonoBehaviour, IMouseOverUI {

    private bool _inUse;
    public Patient guy;

    public bool InUse
    {
        get { return _inUse; }
        set
        {
            _inUse = value;
            if (currPopup == null) return;
            Text t = currPopup.GetComponentInChildren<Text>();
            if (value) {
                t.text = "Requires Materials!\n";
                foreach (string item in guy.data.required_items) {
                    t.text += item + "\n";
                }
                t.color = new Color(1, 0, 0);
            } else {
                t.text = "Available";
                t.color = new Color(0, 1, 0);
            }
        }
    }
    public RectTransform popupPrefab;
    private RectTransform currPopup;

    public void ShowUI(Transform parent, Camera camera)
    {
        if (currPopup != null)
        {
            HideUI();
        }
        if (popupPrefab != null)
        {
            currPopup = Instantiate(popupPrefab);
            currPopup.transform.SetParent(parent);
            InUse = InUse;
        }
    }

    public void UpdateUI(Camera camera, Vector3 point)
    {
        if (currPopup != null)
        {
            currPopup.transform.position = RectTransformUtility.WorldToScreenPoint(camera, point);
        }
    }

    public void HideUI()
    {
        if (currPopup != null)
        {
            Destroy(currPopup.gameObject);
        }
    }

    public void OnClick(int button = 0)
    {
        if (Player.inst.escortee != null && !_inUse) {
            guy = Player.inst.escortee;
            Player.inst.escortee.follow = false;
            Player.inst.escortee.destination = transform.position;
            Player.inst.escortee = null;
        }
    }
}
