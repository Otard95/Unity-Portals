using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerLook : MonoBehaviour {

	[SerializeField] Camera playerCam;
	[SerializeField] float sensitivity;

	Rigidbody _rb;

	void Start () {
		Cursor.lockState = CursorLockMode.Locked;
		_rb = GetComponent<Rigidbody>();
	}
	
	void Update () {

		float y_rot = Input.GetAxisRaw("Mouse X");
		float x_rot = Input.GetAxisRaw("Mouse Y");

		y_rot *= sensitivity;
		x_rot *= sensitivity;

		Vector3 player_rot = new Vector3(0f, y_rot, 0f);
		Vector3 cam_rot = new Vector3(-x_rot, 0f, 0f);

		_rb.MoveRotation(_rb.rotation * Quaternion.Euler(player_rot));

		if (playerCam != null) {
			playerCam.transform.Rotate(cam_rot);
		}

	}

}
