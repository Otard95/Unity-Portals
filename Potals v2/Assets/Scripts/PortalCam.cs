using UnityEngine;


public class PortalCam : MonoBehaviour {

	public Transform playerCamera;
	public Transform portal;
	public Transform otherPortal;
	
	// Update is called once per frame
	void LateUpdate () {

		// Get player pos relative to other portal
		Vector3 player_portal_offset = playerCamera.position - otherPortal.position;

		float rel_rot = 180 + Vector3.Angle(otherPortal.forward, portal.forward);
		Vector3 normal = Vector3.Cross(otherPortal.forward, portal.forward);
		normal = (normal == Vector3.zero) ? Vector3.up : normal;

		Quaternion portal_rot_diff = Quaternion.AngleAxis(rel_rot, normal);
		player_portal_offset = portal_rot_diff * player_portal_offset;
		transform.position = portal.position + player_portal_offset;

		Vector3 new_cam_dir = portal_rot_diff * playerCamera.forward;
		transform.rotation = Quaternion.LookRotation(new_cam_dir, Vector3.up);

	}

}
