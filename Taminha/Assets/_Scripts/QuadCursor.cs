using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Taminha {
	public class QuadCursor:MonoBehaviour {
		public Camera cam;
		public bool show = false;
		public float alpha = 0;
		
		public Transform tr { get; private set; }
		Renderer rend;
		Color colour;
		
		void Awake() {
			tr = transform;
			rend = GetComponent<Renderer>();
			tr.position = new Vector3(0,0,-9.9f);
			colour = rend.material.color;
		}

		void Update() {
			if (show) {
				if (alpha < 1) {
					alpha += Time.deltaTime*4;
					if (alpha > 1) alpha = 1;
					rend.material.color = new Color(colour.r,colour.g,colour.b,alpha);
				}
				float h = cam.orthographicSize;
				float w = h*CameraController.idealProp;
				float x = cam.transform.position.x;
				float y = cam.transform.position.y;
				var px = Mathf.Clamp(tr.position.x+GameController.mouseDelta.x*h,x-w,x+w);
				var py = Mathf.Clamp(tr.position.y+GameController.mouseDelta.y*h,y-h,y+h);
				tr.position = new Vector3(px,py,tr.position.z);
			} else {
				if (alpha > 0) {
					alpha -= Time.deltaTime*4;
					if (alpha < 0) alpha = 0;
					rend.material.color = new Color(colour.r,colour.g,colour.b,alpha);
				}
			}
		}
	}
}