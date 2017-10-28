using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Taminha {
	public class MeshDeformerController:MonoBehaviour {
		public Camera cam;
		public MeshDeformer mesh;
		public Transform cursor;
		public bool useTimer = true;
		public float timer = 0;
		public float scoreGoal = .9f;
		
		public bool done = false;

		Vector2 cursorPos;
		Vector2 prevPos;
		int mouseState = 0;
		bool deforming = false;
		bool dragging = false;
		float resizing = 1;
		float resetSeed = 0;
		Vector3 one;
		
		void Start() {
			cursorPos = Vector2.zero;
			one = mesh.transform.localScale;
			mesh.spawnPos = mesh.transform.InverseTransformPoint(transform.position);
		}

		void Update() {
			cursorPos += GameController.mouseDelta;
			float h = cam.orthographicSize;
			float w = h*CameraController.idealProp;
			if (cursorPos.x < -w) cursorPos.x = -w;
			if (cursorPos.x > w) cursorPos.x = w;
			if (cursorPos.y < -h) cursorPos.y = -h;
			if (cursorPos.y > h) cursorPos.y = h;
			cursor.localPosition = new Vector3(cursorPos.x,cursorPos.y,cursor.localPosition.z);
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
			if (mouseState == 1) {
				var pos = (Vector2)mesh.transform.InverseTransformPoint(cursorPos);
				if (!deforming) {
					deforming = true;
					prevPos = pos;
				} else if (prevPos != pos) {
					if (mesh.Deform(prevPos,pos-prevPos)) apply = true;
					prevPos = pos;
				}
			} else if (deforming) {
				deforming = false;
				updateScore = true;
			}
			if (mouseState == 2) {
				var pos = (Vector2)mesh.transform.InverseTransformPoint(cursorPos);
				if (!dragging) {
					dragging = true;
					prevPos = pos;
				} else if (prevPos != pos) {
					mesh.Move(pos-prevPos);
					apply = true;
					prevPos = pos;
				}
			} else if (dragging) {
				dragging = false;
				updateScore = true;
			}
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
		}
	}
}