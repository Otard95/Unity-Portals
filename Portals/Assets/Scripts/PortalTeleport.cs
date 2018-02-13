using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleport : MonoBehaviour {

	[SerializeField] Transform target;

	bool _player_intersecting = false;
	Transform _player = null;

	void Update () {

		if (!_player_intersecting) return;

		Vector3 portal_to_player = _player.position - transform.position;

		if (Vector3.Dot(transform.forward, portal_to_player) >= 0f) return;

		float portals_rel_angle = -Quaternion.Angle(target.rotation, transform.rotation);
		portals_rel_angle += 180;

		_player.Rotate(Vector3.up, portals_rel_angle);
		_player.position = target.position + portal_to_player;

		_player_intersecting = false;
		_player = null;

	}
	
	void OnTriggerEnter (Collider other) {

		if (!other.CompareTag("Player")) return;

		_player_intersecting = true;
		_player = other.transform;

	}

	void OnTriggerExit (Collider other) {

		if (!other.CompareTag("Player")) return;

		_player_intersecting = false;
		_player = null;

	}

}
