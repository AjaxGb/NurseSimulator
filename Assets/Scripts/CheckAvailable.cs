using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class CheckAvailable : MonoBehaviour, IMouseOverUI {
    
    public Patient guy;
    public Transform bedLocation;
    private bool Treating = false;
    private float TreatingTimeLeft = 999f;

    public bool InUse
    {
        get { return guy != null; }
    }
    public RectTransform popupPrefab;
    public RectTransform currPopup;

    public void Update()
    {
        Text t = null;
        if (currPopup != null)
        {
            t = currPopup.GetComponentInChildren<Text>();
        }
        if (guy != null && guy.data.requiredItemTypes.Count <= 0 && !Treating)
        {
            Treating = true;
            TreatingTimeLeft = guy.data.treatment_time;
            t.text = "Operating on Patient";
            t.color = new Color(1, 1, 0);
        }
        if (Treating)
        {
            TreatingTimeLeft -= Time.deltaTime;
            if (TreatingTimeLeft <= 0)
            {
                Destroy(guy.gameObject);
                guy = null;
                t.text = "Available, just cured Patient";
                t.color = new Color(0, 1, 0);
                Treating = false;
            }
        }
    }

    public void UpdateText()
    {
        if (!Treating)
        {
            if (currPopup == null) return;
            Text t = currPopup.GetComponentInChildren<Text>();
            if (InUse)
            {
                t.text = "Requires Materials!\n";
                foreach (var item in guy.data.requiredItemTypes)
                {
                    t.text += item.name + "\n";
                }
                t.color = new Color(1, 0, 0);
            }
            else
            {
                t.text = "Available";
                t.color = new Color(0, 1, 0);
            }
        }
    }

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
            UpdateText();
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
        if (InUse)
        {
            //Check inventory for needed items and remove them
            guy.data.requiredItemTypes.RemoveAll(item => Player.inst.inventory.RemoveCount(item) > 0);
        } else if (Player.inst.escortee != null) {
            guy = Player.inst.escortee;
			guy.room = this;
            guy.follow = false;
			guy.destination = bedLocation.position;
            Player.inst.escortee = null;
        }
        UpdateText();
    }
}
