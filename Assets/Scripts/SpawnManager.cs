using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnManager : MonoBehaviour {
	[Tooltip("The hitable prefab")]
	[SerializeField] private Hitable _hitablePrefab;

	[Tooltip("A delay time in seconds to wait before start spawning collectables")]
	[SerializeField] private float _startDelay = 2;

	[Tooltip("The time in seconds in which the spawn will keep repeating")]
	[SerializeField] private float _repeatRate = 2;

	[Tooltip("Amount of speed which the hitable object will increase over time")]
	[SerializeField] private float _speedIncrease = .5f;

	private ObjectPool<Hitable> _pool;
	private float _timer = 0f;

	private void Start() {
		_pool = new ObjectPool<Hitable>(CreateHitable, OnGetHitableFromPool, OnReleaseHitableToPool,
		hitable => Destroy(hitable.gameObject), true, 6, 12);

		_timer = _startDelay;
	}

	private void Update() {
		if (GameManager.Instance.isGameRunning)
			CallRepeating(SpawnHitable);
	}

	private void CallRepeating(Action action) {
		_timer -= Time.deltaTime;
		if (_timer < 0) {
			_timer = _repeatRate;
			action();
		}
	}

	private Hitable CreateHitable() {
		return Instantiate(_hitablePrefab, CreateRandomPosition(_hitablePrefab), _hitablePrefab.transform.rotation);
	}

	private Vector3 CreateRandomPosition(Hitable hitable) {
		return hitable.CreateRandomPosition();
	}

	private void OnGetHitableFromPool(Hitable hitable) => hitable.gameObject.SetActive(true);

	private void OnReleaseHitableToPool(Hitable hitable) {
		hitable.transform.position = CreateRandomPosition(hitable);
		hitable.gameObject.SetActive(false);
	}

	private void SpawnHitable() {
		Hitable hitable = _pool.Get();
		hitable.moveLeftVelocity += _speedIncrease;
		hitable.GetComponent<Hitable>().SetKill(Kill);
	}

	private void Kill(Hitable hitable) => _pool.Release(hitable);
}
