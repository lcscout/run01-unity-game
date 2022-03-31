using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using CandyCoded.env;

public class AdsManager : MonoBehaviour, IUnityAdsListener {
	public static AdsManager Instance;
	public static event Action OnAdsFinished;

	private string _gameId = "";

	private void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}

	private void Start() {
		env.TryParseEnvironmentVariable("GAME_ID_ANDROID", out string _gameId);

		Advertisement.Initialize(_gameId);
		Advertisement.AddListener(this);
	}

	public void PlayAd() {
		if (Advertisement.IsReady("Interstitial_Android"))
			Advertisement.Show("Interstitial_Android");
	}

	public void OnUnityAdsDidFinish(string placementId, ShowResult showResult) => OnAdsFinished?.Invoke();

	public void OnUnityAdsDidError(string message) { }

	public void OnUnityAdsDidStart(string placementId) { }

	public void OnUnityAdsReady(string placementId) { }
}
