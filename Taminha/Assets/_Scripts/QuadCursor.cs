using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Taminha {
	public class QuadCursor:MonoBehaviour {
		public Camera cam;
		public bool show = false;
		public bool transparent = false;
		public float alpha = 0;
		public Texture2D[] tex;
		
		public Transform tr { get; private set; }
		Renderer rend;

		bool dialogueMode = false;
		Vector2 velocity;
		float velocityTempo;
		
		void Awake() {
			tr = transform;
			rend = tr.Find("Quad").GetComponent<Renderer>();
			tr.position = new Vector3(0,0,-9.9f);
		}

		void Update() {
			if (show) {
				if (alpha < 1) {
					alpha += Time.deltaTime*4;
					if (alpha > 1) alpha = 1;
				}
				float h = cam.orthographicSize;
				float w = h*CameraController.idealProp;
				if (dialogueMode) {
					float t = GameController.deformScore*GameController.deformScore;
					velocityTempo -= Time.deltaTime;
					if (velocityTempo <= 0) {
						velocityTempo = .03f;
						velocity = Random.insideUnitCircle;
						velocity.y *= .75f;
						velocity.x -= (1-t)*1.75f;
					}
					float px = tr.localPosition.x;
					float py = tr.localPosition.y;
					float struggleFactor = Mathf.Lerp(.35f,.1f,t)*Time.deltaTime*3*(1+GameController.mouseDelta.magnitude*.5f);
					float controlFactor = Mathf.Lerp(.4f,.9f,t);
					px += velocity.x*struggleFactor;
					py += velocity.y*struggleFactor;
					px += Sqrt(GameController.mouseDelta.x)*.05f*controlFactor;
					py += Sqrt(GameController.mouseDelta.y)*.03f*controlFactor;
					tr.localPosition = new Vector3(Mathf.Clamp(px,-w,w),Mathf.Clamp(py,-5,-3),tr.localPosition.z);
				} else {
					float px = tr.position.x;
					float py = tr.position.y;
					float x = cam.transform.position.x;
					float y = cam.transform.position.y;
					px += GameController.mouseDelta.x*h;
					py += GameController.mouseDelta.y*h;
					tr.position = new Vector3(Mathf.Clamp(px,x-w,x+w),Mathf.Clamp(py,y-h,y+h),tr.position.z);
				}
			} else {
				if (alpha > 0) {
					alpha -= Time.deltaTime*4;
					if (alpha < 0) alpha = 0;
				}
			}
			if (alpha > 0) {
				rend.material.mainTexture = tex[(int)(Time.time*12)%tex.Length];
			}
			rend.material.color = new Color(1,1,1,(3-2*alpha)*alpha*alpha*(transparent ? .5f : 1));
		}

		public void SetDialogue() {
			rend.gameObject.layer = 9;
			dialogueMode = true;
		}

		float Sqrt(float x) {
			if (x == 0) return 0;
			if (x < 0) return -Mathf.Sqrt(-x);
			return Mathf.Sqrt(x);
		}
	}
}