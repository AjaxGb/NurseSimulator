using UnityEngine;

public class PopupManager : MonoBehaviour {
	public Camera playerCamera;
	public bool useCenterOfCamera;
	public float maxDistance = 10.0f;
	public string mouseOverTag;

	public static PopupManager inst { get; private set; }

	private IMouseOverUI _currMouseOver;
	public IMouseOverUI currMouseOver {
		get { return _currMouseOver; }
		set {
			if (_currMouseOver != null) {
				_currMouseOver.HideUI();
			}
			_currMouseOver = value;
			if (_currMouseOver != null) {
				_currMouseOver.ShowUI(this.transform, GetActualCamera());
			}
		}
	}

	void Start () {
		if (inst != null) {
			Debug.LogWarning("Two PopupManagers are active at once!");
		} else {
			inst = this;
		}
	}
	
	void Update () {
		Camera cam = GetActualCamera();
		Ray ray = useCenterOfCamera ?
			cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0)) :
			cam.ScreenPointToRay(Input.mousePosition);

		RaycastHit info;
		if (Physics.Raycast(ray, out info, maxDistance) && info.transform.CompareTag(mouseOverTag)) {
			IMouseOverUI foundMouseOver = info.transform.GetComponent<IMouseOverUI>();
			if (foundMouseOver != currMouseOver) {
				currMouseOver = foundMouseOver;
			}
			currMouseOver.UpdateUI(cam, info.point);
			// Do input
			if (Input.GetMouseButtonDown(0)) {
				foundMouseOver.OnClick(0);
			}
			if (Input.GetMouseButtonDown(1)) {
				foundMouseOver.OnClick(1);
			}
		} else {
			currMouseOver = null;
		}
	}

	public Camera GetActualCamera() {
		return (playerCamera != null) ? playerCamera : Camera.main;
	}
}
