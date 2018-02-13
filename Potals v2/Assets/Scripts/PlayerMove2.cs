using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(SphereCollider))]
public class PlayerMove2 : MonoBehaviour {


	#region UnityProporties

	[Header("Movement")]
	[SerializeField] private float moveForce = 50;
	[Tooltip("If player is airborn 'move force' is multiplied by this value")]
	[Range(0.1f,1)]
	[SerializeField] private float airbornForceMultiplyer = 0.2f;
	[Tooltip("Persent of the 'move force' that gets applied " +
		"based on players current speed")]
	[SerializeField] private AnimationCurve speedGain;
	[SerializeField] private float maxSpeed = 5;
	[Tooltip("Maximum speed in air, after a jump from standstill")]
	[SerializeField] private float stillJumpMaxSpeed = 3;
	[SerializeField] private float jumpForce = 6;
	[Tooltip("The minimum amount of time(s) befor you can jump again.")]
	[Range(.1f, 1)]
	[SerializeField] private float minimumJumpIterval = .2f;
	[Tooltip("Gravity while falling = gravity + gravity * <this value>")]
	[Range(0,1)]
	[SerializeField] private float gravityMutiplyer = .5f;
	[Tooltip("If velocity and facing vector is aligned and " +
		"jump immediatly after landing, gain a speed boost.")]
	[Range(0,10)]
	[SerializeField] private float bunnyHopBoost = 2.5f;

	[Header("Physics")]
	[Range(0, 0.4f)]
	[SerializeField] private float groundCheckOffset = .01f;
	[Range(0,8)]
	[SerializeField] private float groundedFriction = 4;
	[Range(0,4)]
	[SerializeField] private float airbornFriction = 0;
	[Tooltip("Switch back to 'grounded friction' after being grounded " +
		"for this delay(seconds). This will affect BHopp boost too.")]
	[Range(.1f, 1)]
	[SerializeField] private float switchBackDely = .25f;
	[Tooltip("If velocity and facing direction is with in this angle(deg), " +
		"they are considered aligned.")]
	[Range(0,45)]
	[SerializeField] private float forwardAngleAlignmentOffset = 15;

	#endregion

	#region PrivateFields

	// Components
	private Rigidbody _rb;
	private SphereCollider _collider;
	//private Animator anim;

	// Fields
	private bool _is_grounded = false;
	private float _last_jump;
	private bool _is_still_jump = false;
	// Last forward facing vector of the transform ignoring vertical axis
	private Vector2 _last_forward;

	#endregion

	// Use this for initialization
	void Start() {
		_rb = GetComponent<Rigidbody>();
		_collider = GetComponent<SphereCollider>();
		//anim = GetComponent<Animator> ();
		_last_forward = new Vector2 (transform.forward.x, transform.forward.z);
	}

	// Update is called once per frame
	void Update() {

		//_is_grounded = GroundCheck();

		Movement();

		RotateVelocity();

		Jump();

		/**
		 * ## Fast fall
		 * if player is falling apply extra gravity.
		 * physically correct gravity while falling
		 * FEELS fake
		*/
		if (_rb.velocity.y < 0) {
			_rb.AddRelativeForce(Physics.gravity * gravityMutiplyer);
		}

		// Update _last_forward ignoring vertical component
		_last_forward.x = transform.forward.x;
		_last_forward.y = transform.forward.z;

	}

	void OnCollisionStay (Collision col) {

		if (col.collider.CompareTag("Ground")) {
			_is_grounded = true;
			_is_still_jump = false;
			Invoke("ResetColliderFriction", switchBackDely);
		}

	}

	void OnCollisionExit (Collision col) {

		if (col.collider.CompareTag("Ground")) {
			_is_grounded = false;
			_collider.material.dynamicFriction = airbornFriction;
		}

	}

	private bool GroundCheck() {

		// player can not become grounded until at least
		// minimumJumpIterval seconds has passed since last jump
		if (Time.unscaledTime <= (_last_jump + minimumJumpIterval))
			return false;

		Ray ground_check = new Ray(
				transform.position + _collider.center,
				Vector3.down
			);

		RaycastHit hit;
		var rc_res = Physics.SphereCast(
				ground_check,
				_collider.radius,
				out hit,
				_collider.radius + groundCheckOffset
			);

		if (rc_res) {
			if (!hit.collider.CompareTag("Player")) {

				// Reset fields relavant for airborn events
				_is_still_jump = false;
				//anim.SetBool ("Jump", false);
				Invoke("ResetColliderFriction", switchBackDely);
				
				return true;
			}
		}

		_collider.material.dynamicFriction = airbornFriction;

		return false;

	}

	private void ResetColliderFriction () {
		_collider.material.dynamicFriction = groundedFriction;
	}

	/**
	 * ## Get players inputs and handle the movement 
	*/
	private void Movement () {

		Vector3 input_dir = GetPlayerInput();

		// limit the movement force by the maximum player speed
		// applied over a curve.
		float movent_mult = speedGain.Evaluate(
			_rb.velocity.magnitude / (_is_still_jump ? stillJumpMaxSpeed : maxSpeed)
		);

		var movement_force = new Vector3(
				input_dir.x * moveForce,
				0,
				input_dir.z * moveForce
			);
		movement_force *= movent_mult;
		if (!_is_grounded) movement_force *= airbornForceMultiplyer;

		// Apply the force to the Rigidbody
		_rb.AddRelativeForce(movement_force, ForceMode.Force);

	}

	/**
	 * ## Checks if velocity is aligned with provided vector
	*/
	private bool VelocityAlignedWith(Vector2 v) {

		// We don't care about the vertical velocity
		Vector2 vel = new Vector2(_rb.velocity.x, _rb.velocity.z);

		// Find angle between prev-forward and vel
		// angle = acos(prev*vel / prev.mag * vel.mag)
		// prev.mag = 1 (unit-vector)
		float dot = Vector2.Dot(vel, v);
		dot = Mathf.Clamp(dot / vel.magnitude, -1, 1); // clamp due to rounding error
		float angle = (180 / Mathf.PI) * Mathf.Acos(dot);

		// if angle <= forwardAngleAlignmentOffset => rotate velosity angle
		return angle <= forwardAngleAlignmentOffset;

	}

	/*
	 * ## Rotate player velocity with its roataion
	*/
	private void RotateVelocity () {

		// only relevant while airborn
		if (_is_grounded) return;

		// We don't care about the vertical velocity
		Vector2 vel = new Vector2(_rb.velocity.x, _rb.velocity.z);
		
		if (VelocityAlignedWith(_last_forward)) {
			// vel += current_forward - prev * vel.mag
			Vector2 forward = new Vector2(transform.forward.x, transform.forward.z);

			vel += (forward - _last_forward) * vel.magnitude;

			_rb.velocity = new Vector3(vel.x, _rb.velocity.y, vel.y);

		}

	}

	/**
	 * ## Evaluate jump condidtions and execute a jump
	*/
	private void Jump () {

		if (Input.GetButton("Jump") && _is_grounded ) {

			if (_rb.velocity.sqrMagnitude < .1) {
				/*
				 * if player jumps while standing still
				 * max speed in the air is limited
				*/
				_is_still_jump = true;
			}

			//anim.SetBool ("Jump", true);
			// prevents the downwards momentum from previos jumps
			// affecting this one
			_rb.velocity.Set(_rb.velocity.x, 0, _rb.velocity.z);
			_rb.AddRelativeForce(0, jumpForce, 0, ForceMode.Impulse);

			// Evaluate BHopp
			BunnyHopp();

			// prevent multiple jumps within the frames where the
			// margin of error for the ground check may say the player
			// is still grounded
			_last_jump = Time.unscaledTime;
			_is_grounded = false;
			_collider.material.dynamicFriction = airbornFriction;

		}

	}

	/**
	 * ## Evaluate BHopp condidtions and add force
	*/
	private void BunnyHopp () {

		//Debug.Log("Aligned: " + VelocityAlignedWith(_last_forward));
		//Debug.Log("Timing: " + _collider.material.dynamicFriction);

				// If player is moving forward
		if (VelocityAlignedWith(_last_forward) &&
				// And jumps befor friction is reset
				_collider.material.dynamicFriction == airbornFriction)
		{

			// Get the forward facing vector and multiply by bunnyHoppBoost
			Vector3 boost = transform.forward * bunnyHopBoost;
			//Debug.DrawRay(transform.position, boost, Color.red);

			// And add it as a force on _rb
			_rb.AddForce(boost, ForceMode.Impulse);

			Debug.Log("bhop");

		}

	}

	private Vector3 GetPlayerInput () {

		var input_dir = new Vector3(
				Input.GetAxisRaw("Horizontal"),
				0,
				Input.GetAxisRaw("Vertical")
			);
		// The input vectors magnitude should never exceed 1
		return input_dir.normalized;

	}

}
