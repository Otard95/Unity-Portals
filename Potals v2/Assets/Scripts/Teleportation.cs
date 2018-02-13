using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportation : MonoBehaviour {

	public Transform target;

	bool _player_overlapping = false;
	Transform _player = null;

	void Update () {

		if (!_player_overlapping) return;

		Vector3 portal_to_player = _player.position - transform.position;
		float dot = Vector3.Dot(transform.forward, portal_to_player);

		if (dot < 0f) return;
		
		float rel_rot = 180 - Vector3.Angle(target.forward, transform.forward);
		Vector3 normal = Vector3.Cross(target.forward, transform.forward).normalized;
		normal = (normal == Vector3.zero) ? Vector3.up : normal;

		Quaternion portal_rot_diff = Quaternion.AngleAxis(rel_rot, normal);
		Debug.Log(portal_to_player.z);
		portal_to_player = portal_rot_diff * portal_to_player;
		_player.position = target.position + portal_to_player;
		Debug.Log(portal_to_player.z);

		//Vector3 new_player_look_dir = portal_rot_diff * _player.forward;
		//_player.rotation = Quaternion.LookRotation(new_player_look_dir);
		//_player.rotation = _player.rotation * portal_rot_diff;
		_player.Rotate(normal, rel_rot);

		_player_overlapping = false;
		_player = null;

	}

	void OnTriggerEnter (Collider other) {
		// if collider is not player ignore it
		if (other.CompareTag("Player")) {
			_player_overlapping = true;
			_player = other.transform;
		}

	}

	void OnTriggerExit (Collider other) {
		// if collider is not player ignore it
		if (other.CompareTag("Player")) {
			_player_overlapping = false;
			_player = null;
		}
	}

}
