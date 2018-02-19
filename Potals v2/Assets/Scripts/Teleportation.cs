using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportation : MonoBehaviour {

	public Transform target;

	bool _player_overlapping = false;
	Transform _player = null;
	Rigidbody _player_rb = null;
	bool _entered_from_back = false;

	void Update () {

		if (!_player_overlapping) return;

		// determin what side of the trigger the player is on
		Vector3 portal_to_player = _player.position - transform.position;
		float dot = Vector3.Dot(transform.forward, portal_to_player);

		// if player entered from back but has passed to the other side reset the flag
		if (_entered_from_back && dot < 0) _entered_from_back = false;

		// if player actually passed the teleport trigger. (did not enter from back)
		if (_entered_from_back ||  dot < 0f) return;
		
		// Calculate the rotation between portlas and the axis to rotate around
		float rel_rot = 180 - Vector3.Angle(target.forward, transform.forward);
		Vector3 normal = Vector3.Cross(target.forward, transform.forward);
		normal = (normal == Vector3.zero) ? Vector3.up : normal;

		// Rotate the players relative position to the entering portal and
		// move the player to the other portal using the relative position as the offset
		Quaternion portal_rot_diff = Quaternion.AngleAxis(rel_rot, normal);
		portal_to_player = portal_rot_diff * portal_to_player;
		_player.position = target.position + portal_to_player;

		// rotate the player
		_player.Rotate(normal, rel_rot);

		// if the player has a rigidbody rotate the velocity
		if (_player_rb != null) {
			_player_rb.velocity = portal_rot_diff * _player_rb.velocity;
		}

		// reset flaggs and refrences
		_player_overlapping = false;
		_player = null;
		_player_rb = null;

	}

	void OnTriggerEnter (Collider other) {
		// if collider is not player ignore it
		if (other.CompareTag("Player")) {
			_player_overlapping = true;
			_player = other.transform;
			_player_rb = _player.GetComponent<Rigidbody>();

			// check if player entered from back
			Vector3 portal_to_player = _player.position - transform.position;
			float dot = Vector3.Dot(transform.forward, portal_to_player);

			_entered_from_back = (dot > 0f);

		}

	}

	void OnTriggerExit (Collider other) {
		// if collider is not player ignore it
		if (other.CompareTag("Player")) {
			_player_overlapping = false;
			_player = null;
			_player_rb = null;
		}
	}

}
