using UnityEngine;

public class PopupManager : MonoBehaviour {
	public Camera playerCamera;
	public bool useCenterOfCamera;
	public float maxDistance = 10.0f;
	public LayerMask layerMask;

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
		if (Physics.Raycast(ray, out info, maxDistance, layerMask)) {
			IMouseOverUI foundMouseOver = info.transform.GetComponent<IMouseOverUI>();
			if (foundMouseOver == currMouseOver) {
				currMouseOver.UpdateUI(cam);
			} else {
				currMouseOver = foundMouseOver;
			}
		} else {
			currMouseOver = null;
		}
	}

	public Camera GetActualCamera() {
		return (playerCamera != null) ? playerCamera : Camera.main;
	}
}
