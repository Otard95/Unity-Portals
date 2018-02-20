using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour {

	#region SINGLETON PATTERN

	public static PortalManager _instance;
	public static PortalManager Instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<PortalManager>();

				if (_instance == null) {
					GameObject container = new GameObject("PortalManager");
					_instance = container.AddComponent<PortalManager>();
				}
			}

			return _instance;
		}
	}

	void Awake () {
		if (portalA == null || portalB == null) {
			Debug.LogError(transform.name + " => PortalManager: " +
				"Missing one or more portal references");
		}
	}

	#endregion

	#region Unity Fields

	[SerializeField]
	Transform portalA;
	[SerializeField]
	Transform portalB;
	[SerializeField]
	string cameraAIgnoreLayer;
	[SerializeField]
	string cameraBIgnoreLayer;
	[SerializeField]
	float offset = 0.001f;

	#endregion

	int camAIgnore;
	int camBIgnore;
	PortalSurface _portalableA;
	PortalSurface _portalableB;

	void Start () {
		camAIgnore = LayerMask.NameToLayer(cameraAIgnoreLayer);
		camBIgnore = LayerMask.NameToLayer(cameraBIgnoreLayer);
	}

	public void PlaceA (RaycastHit ray_hit) {

		// place portalA
		if (ray_hit.collider.CompareTag("Portalable")) {
			//Fix colliders and layermask
			PortalSurface new_portalable = new PortalSurface(ray_hit.transform.gameObject, ray_hit.collider, ray_hit.transform.gameObject.layer);
			SwapPortalable(_portalableA, new_portalable, camAIgnore);
			_portalableA = new_portalable;
			// Get portals new position and roation
			// Temp fix
			Vector3 off = ray_hit.transform.up + ray_hit.transform.right / 2;
			Vector3 position = ray_hit.transform.position + off + (ray_hit.transform.forward * offset);
			Quaternion rotation = Quaternion.LookRotation(ray_hit.transform.forward, Vector3.up);
			// Apply position and rotation
			Place(portalA, position, rotation);
		}

	}

	public void PlaceB (RaycastHit ray_hit) {

		// place portalA
		if (ray_hit.collider.CompareTag("Portalable")) {
			//Fix colliders
			PortalSurface new_portalable = new PortalSurface(ray_hit.transform.gameObject, ray_hit.collider, ray_hit.transform.gameObject.layer);
			SwapPortalable(_portalableB, new_portalable, camBIgnore);
			_portalableB = new_portalable;
			// Get portals new position and roation
			// Temp fix
			Vector3 off = ray_hit.transform.up + ray_hit.transform.right / 2;
			Vector3 position = ray_hit.transform.position + off + (ray_hit.transform.forward * offset);
			Quaternion rotation = Quaternion.LookRotation(ray_hit.transform.forward, Vector3.up);
			// Apply position and rotation
			Place(portalB, position, rotation);
		}

	}

	void Place (Transform portal, Vector3 position, Quaternion rotation) {

		portal.position = position;
		portal.rotation = rotation;

	}

	void SwapPortalable (PortalSurface oldPortalable, PortalSurface newPortalable, int mask) {

		if (oldPortalable.collider != null) {
			oldPortalable.collider.enabled = true;
			oldPortalable.gameObject.layer = oldPortalable.layerMask;
		}

		newPortalable.collider.enabled = false;
		newPortalable.gameObject.layer = mask;

	}

}

public struct PortalSurface {

	public GameObject gameObject;
	public Collider collider;
	public int layerMask;

	public PortalSurface (GameObject obj, Collider col, int mask) {
		gameObject = obj;
		collider = col;
		layerMask = mask;
	}

}