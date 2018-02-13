using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamControll : MonoBehaviour {

	[SerializeField] Transform portal;
	[SerializeField] Transform otherPortal;
	[SerializeField] Transform playerCam;
	
	void LateUpdate () {
		
		float portals_rel_angle = Quaternion.Angle(otherPortal.rotation, portal.rotation);

		Vector3 target_rel_pos = Quaternion.Euler(0, -portals_rel_angle, 0) * (playerCam.position - otherPortal.position);
		transform.position = portal.position + target_rel_pos;

		Vector3 look_dir = Quaternion.Euler(0, -portals_rel_angle, 0) * playerCam.forward;
		transform.rotation = Quaternion.LookRotation(look_dir, Vector3.up);

	}

}
