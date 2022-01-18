using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
	public static bool IsGrounded { get; private set; }

	[Header("Ground Settings")]
	[Tooltip("A position marking where to check if the player is grounded")]
	[SerializeField] private Transform _groundCheck;

	[Tooltip("A mask determining what is ground to the character")]
	[SerializeField] private LayerMask _groundLayerMask;

	[Header("Movement Settings")]
	[Space]
	[Tooltip("Amount of force added when the player jumps")]
	[SerializeField] private float _jumpForce = 10f;

	[Header("Components Reference")]
	[Space]
	[SerializeField] private CameraController _camera;

	private const float _GROUND_CHECK_RADIUS = .2f;

	private float _airTime = 0f;
	private int _gravityDirection = 1;
	private int _jumps = 1;

	private void OnEnable() {
		// InputsController.OnTouchInput += Jump;
		InputsController.OnJump += Jump;
		InputsController.OnMove += VerifyMove;
	}

	private void OnDisable() {
		// InputsController.OnTouchInput -= Jump;
		InputsController.OnJump -= Jump;
		InputsController.OnMove -= VerifyMove;
	}

	private void Start() => ResetGravity();

	private void FixedUpdate() {
		if (!GameManager.Instance.isGameRunning || GameManager.Instance.isGamePaused)
			return;

		OnLanding();
		OnGround();
		OnAir();
	}

	public void ButtonJump() {
		if (_jumps > 0 && GameManager.Instance.isGameRunning && !GameManager.Instance.isGamePaused) {
			GetComponent<Rigidbody>().AddForce(new Vector3(0, _jumpForce * _gravityDirection, 0), ForceMode.VelocityChange);
			_jumps--;
		}
	}

	public void ButtonMove() {
		if (IsGrounded && GameManager.Instance.isGameRunning && !GameManager.Instance.isGamePaused) {
			InvertPosition();
			_gravityDirection *= -1;
			_camera.InvertCamera();
		}
	}

	private void Jump() {
		if (_jumps > 0) {
			GetComponent<Rigidbody>().AddForce(new Vector3(0, _jumpForce * _gravityDirection, 0), ForceMode.VelocityChange);
			_jumps--;
		}
	}

	private void OnLanding() {
		if (_airTime > 0 && IsGrounded) {
			RechargeJumps();
			_airTime = 0;
		}
	}

	// To ensure the jump will be called only one time
	private void RechargeJumps() => _jumps = 1;

	private void OnGround() => IsGrounded = Physics.CheckSphere(_groundCheck.position, _GROUND_CHECK_RADIUS, _groundLayerMask);

	private void OnAir() {
		if (!IsGrounded)
			_airTime += Time.deltaTime;
	}

	private void VerifyMove(float yDrag) {
		int newGravityDirection = (yDrag > 0) ? 1 : -1;
		if (_gravityDirection != newGravityDirection && IsGrounded) {
			InvertPosition();
			AssignNewGravityDirection(newGravityDirection);
		}
	}

	private void AssignNewGravityDirection(int newGravityDirection) => _gravityDirection = newGravityDirection;

	private void InvertPosition() {
		transform.Rotate(new Vector3(180, 0, 0), Space.Self);
		transform.position = new Vector3(transform.position.x, transform.position.y * -1, transform.position.z);

		InvertGravity();
		RechargeJumps(); // Just in case cuz sometimes the onGround wont recharge
	}

	private void InvertGravity() => Physics.gravity *= -1;

	private void ResetGravity() {
		if (Physics.gravity.y > 0)
			InvertGravity();
	}
}
