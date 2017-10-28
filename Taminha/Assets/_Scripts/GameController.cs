using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Taminha {
	public class GameController:MonoBehaviour {
		public static float deformScore = 0;
		public static Vector2 mouseDelta;

		void Awake() {
			DontDestroyOnLoad(gameObject);
		}

		void Start() {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		void Update() {
			const float vel = .5f;
			mouseDelta = new Vector2(Input.GetAxis("x")*vel,Input.GetAxis("y")*vel);
		}
	}
}
