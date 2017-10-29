using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Melancia.Taminha {
	public class GameController:MonoBehaviour {
		public static float deformScore = 0;
		public static Vector2 mouseDelta;

		static Transform tr;

		static Transform transitionTr;
		static Transform lensTr;
		static Renderer fadeRend;

		static int sceneIndex = 0;
		static int nextScene = -1;
		static float lensTransition = 0;
		static bool fading = false;
		static float fadeTempo = 0;
		static float fadeFromVel = 0;
		static float fadeToVel = 0;

		public static void NextScene(Transition transition = Transition.None,float durationFrom = 0,float durationTo = 0) {
			GoToScene(sceneIndex+1,transition,durationFrom,durationTo);
		}
		
		public static void GoToScene(int index,Transition transition = Transition.None,float durationFrom = 0,float durationTo = 0) {
			switch (transition) {
				case Transition.None:
					nextScene = -1;
					SceneManager.LoadScene(index);
					sceneIndex = index;
					break;
				case Transition.Lens:
					lensTransition = 1;
					nextScene = index;
					lensTr.gameObject.SetActive(true);
					break;
				case Transition.Fade:
					if (Mathf.Approximately(durationFrom,0)) {
						GoToScene(index);
						if (Mathf.Approximately(durationTo,0)) {
							fadeTempo = 0;
							fadeRend.gameObject.SetActive(false);
						} else {
							fadeToVel = 1/durationTo;
							fadeTempo = 1;
							fadeRend.gameObject.SetActive(true);
						}
					} else {
						fadeFromVel = 1/durationFrom;
						fadeToVel = Mathf.Approximately(durationTo,0) ? 0 : 1/durationTo;
						fading = true;
						nextScene = index;
						fadeTempo = 0;
						fadeRend.gameObject.SetActive(true);
					}
					break;
			}
		}

		void Awake() {
			if (tr != null) {
				Destroy(gameObject);
				return;
			}
			tr = transform;
			DontDestroyOnLoad(gameObject);

			transitionTr = tr.Find("Transition");
			lensTr = transitionTr.Find("Lens");
			fadeRend = transitionTr.Find("Fade").GetComponent<Renderer>();

			sceneIndex = SceneManager.GetActiveScene().buildIndex;

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		void Update() {
			const float vel = .1f;
			mouseDelta = new Vector2(Input.GetAxis("x")*vel,Input.GetAxis("y")*vel);
		}

		void LateUpdate() {
			if (lensTransition > 0) {
				bool before = lensTransition >= .5f;
				lensTransition -= Time.deltaTime*1.5f;
				if (before && lensTransition < .5f) GoToScene(nextScene);
				if (lensTransition <= 0) {
					lensTransition = 0;
					lensTr.gameObject.SetActive(false);
				} else {
					const float rot = 53;
					lensTr.localEulerAngles = new Vector3(0,0,Mathf.Lerp(rot,-rot,lensTransition));
				}
			}
			if (fading) {
				fadeTempo += Time.deltaTime*fadeFromVel;
				if (fadeTempo >= 1) {
					fadeTempo = 1;
					fading = false;
					GoToScene(nextScene);
					if (Mathf.Approximately(fadeToVel,0)) {
						fadeTempo = 0;
						fadeRend.gameObject.SetActive(false);
					}
				}
				fadeRend.material.color = new Color(0,0,0,fadeTempo*fadeTempo);
			} else if (fadeTempo > 0) {
				fadeTempo -= Time.deltaTime*fadeToVel;
				if (fadeTempo <= 0) {
					fadeTempo = 0;
					fadeRend.gameObject.SetActive(false);
				} else {
					fadeRend.material.color = new Color(0,0,0,fadeTempo*fadeTempo);
				}
			}
		}
	}

	public enum Transition {
		None,
		Lens,
		Fade,
	}
}
