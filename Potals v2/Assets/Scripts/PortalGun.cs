using System;
using UnityEngine;

public class PortalGun : MonoBehaviour {

	[SerializeField]
	Transform playerCamera;

	PortalManager _portal_manager;
	
	void Start () {
		if (playerCamera == null) {
			Debug.LogError(transform.name + " => PortalGun: Missing playerCamera");
		}
		_portal_manager = PortalManager.Instance;
	}
	
	void Update () {

		if (Input.GetButtonDown("Fire1")) {
			// Shoot the Blue Portal (portalA)
			RaycastHit hit;
			if (Physics.Raycast(
				playerCamera.position,
				playerCamera.forward,
				out hit,
				Int32.MaxValue))
			{

				_portal_manager.PlaceA(hit);

			}

		}

		if (Input.GetButtonDown("Fire2")) {
			// Shoot the Blue Portal (portalA)
			RaycastHit hit;
			if (Physics.Raycast(
				playerCamera.position,
				playerCamera.forward,
				out hit,
				Int32.MaxValue)) {

				_portal_manager.PlaceB(hit);

			}

		}

	}

}
