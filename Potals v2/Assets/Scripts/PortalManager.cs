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
	float offset = 0.001f;

	#endregion

	Collider _portalableA = null;
	Collider _portalableB = null;

	public void PlaceA (RaycastHit ray_hit) {

		// place portalA
		if (ray_hit.collider.CompareTag("Portalable")) {
			//Fix colliders
			SwapPortalable(_portalableA, ray_hit.collider);
			_portalableA = ray_hit.collider;
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
			SwapPortalable(_portalableB, ray_hit.collider);
			_portalableB = ray_hit.collider;
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

	void SwapPortalable (Collider oldPortalable, Collider newPortalable) {

		if (oldPortalable != null) {
			oldPortalable.GetComponent<Collider>().enabled = true;
		}

		newPortalable.GetComponent<Collider>().enabled = false;

	}

}
