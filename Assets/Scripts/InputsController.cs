using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Profiling;

public class InputsController : MonoBehaviour {
	public static event Action OnTouchInput;
	public static event Action OnJump;
	public static event Action<float> OnMove;

	private float _yDrag = 0f;

	public void TouchInput(InputAction.CallbackContext context) {
		OnTouchInput?.Invoke();
	}

	public void Click(InputAction.CallbackContext context) {
		if (context.performed)
			OnJump?.Invoke();
	}

	public void Drag(InputAction.CallbackContext context) {
		if (context.performed)
			_yDrag = context.ReadValue<Vector2>().y;
		if (context.canceled && _yDrag != 0)
			OnMove?.Invoke(_yDrag);
	}

	public void Jump(InputAction.CallbackContext context) {
		if (context.performed)
			OnJump?.Invoke();
	}

	public void Move(InputAction.CallbackContext context) {
		if (context.performed) {
			_yDrag = context.ReadValue<float>();
			if (_yDrag != 0)
				OnMove?.Invoke(_yDrag);
		}
	}
}
