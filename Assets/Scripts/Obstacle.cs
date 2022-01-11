using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {
	[Tooltip("The velocity which the obstacle will continuously move towards the player")]
	[SerializeField] private float _moveLeftVelocity = 20f;

	private void Update() {
		transform.Translate(Vector3.left * _moveLeftVelocity * Time.deltaTime, Space.World);
	}

	private void OnCollisionEnter(Collision other) {
		if (other.gameObject.CompareTag("Player"))
			Destroy(other.gameObject); // TODO: make gameover
		if (other.gameObject.CompareTag("Obstacle Destroyer"))
			Destroy(gameObject);
	}
}
