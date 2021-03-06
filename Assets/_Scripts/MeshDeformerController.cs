﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Taminha {
	public class MeshDeformerController:MonoBehaviour {
		public Camera cam;
		public MeshDeformer mesh;
		public AudioSource aud;
		public Texture2D bg;
		public QuadCursor cursor;
		public bool useTimer = true;
		public float timer = 0;
		public float scoreGoal = .9f;
		
		public bool done = false;
		
		Vector2 prevPos;
		int mouseState = 0;
		bool deforming = false;
		bool dragging = false;
		float resizing = 1;
		float resetSeed = 0;
		Vector3 one;
		float mouseVelocity = 0;
		
		void Start() {
			one = mesh.transform.localScale;
			mesh.spawnPos = mesh.transform.InverseTransformPoint(transform.position);
			cursor.cam = cam;
			mesh.bg = bg;
			aud.volume = 0;
			aud.Play();
		}
		
		void Update() {
			if (resizing > 0) {
				mouseState = 0;
				resizing -= Time.deltaTime*2;
				if (resizing <= 0) {
					resizing = 0;
					mesh.transform.localScale = one;
				} else {
					mesh.transform.localScale = one*(1+Mathf.Sin(resizing*15)*resizing*.1f);
				}
			} else if (done) {
				mouseState = 0;
			} else if (mouseState == 0) {
				if (Input.GetMouseButton(0)) {
					mouseState = 1;
				} else if (Input.GetMouseButton(1)) {
					mouseState = 2;
				}
			} else if (mouseState == 1) {
				if (!Input.GetMouseButton(0)) mouseState = 0;
			} else if (mouseState == 2) {
				if (!Input.GetMouseButton(1)) mouseState = 0;
			}
			var updateScore = false;
			var apply = false;
			float newVelocity = 0;
			if (mouseState == 1) {
				var pos = (Vector2)mesh.transform.InverseTransformPoint(cursor.tr.position);
				if (!deforming) {
					deforming = true;
					prevPos = pos;
				} else if (prevPos != pos) {
					var delta = pos-prevPos;
					if (mesh.Deform(prevPos,delta)) apply = true;
					prevPos = pos;
					newVelocity = delta.sqrMagnitude/Time.deltaTime;
				}
			} else if (deforming) {
				deforming = false;
				updateScore = true;
			}
			if (mouseState == 2) {
				var pos = (Vector2)mesh.transform.InverseTransformPoint(cursor.tr.position);
				if (!dragging) {
					dragging = true;
					prevPos = pos;
				} else if (prevPos != pos) {
					var delta = pos-prevPos;
					mesh.Move(delta);
					apply = true;
					prevPos = pos;
					newVelocity = delta.sqrMagnitude/Time.deltaTime;
				}
			} else if (dragging) {
				dragging = false;
				updateScore = true;
			}
			mouseVelocity = Mathf.Lerp(mouseVelocity,Mathf.Clamp(newVelocity*15,0,2),Time.deltaTime*10);
			aud.pitch = mouseVelocity*.4f+.4f;
			aud.volume = Mathf.Clamp01(mouseVelocity*4);
			resetSeed -= Time.deltaTime*6;
			if (resetSeed <= 0) {
				do {
					resetSeed++;
				} while (resetSeed <= 0);
				mesh.seed = new Vector3(Random.Range(0,100),Random.Range(0,100));
				if (mesh.ApplyMesh(apply)) resizing = 1;
			} else if (apply) {
				if (mesh.ApplyMesh()) resizing = 1;
			}
			if (!done) {
				if (useTimer) {
					timer -= Time.deltaTime;
					if (timer <= 0) {
						timer = 0;
						GameController.deformScore = mesh.GetScore();
						Debug.Log(GameController.deformScore);
						resizing = 1;
						done = true;
					}
				} else if (updateScore) {
					var score = mesh.GetScore();
					Debug.Log(score);
					if (score >= scoreGoal) {
						resizing = 1;
						done = true;
					}
				}
			}
			cursor.show = !done;
		}
	}
}