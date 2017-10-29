using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Taminha {
	public class PosingBoyController:MonoBehaviour {
		public MeshDeformer deform;
		public PosingBoy boy;
		public QuadCursor cursor;
		public Texture2D bg;
		public Texture2D[] tex;

		void Start() {
			var pos = transform.position;
			boy.boy.transform.position = new Vector3(pos.x,pos.y,0);
			boy.tex = tex;
			if (deform != null) {
				boy.bg.enabled = false;
				var s = boy.boy.transform.localScale;
				s = new Vector3(s.x*1.8f,s.y*1.8f,1);
				boy.boy.transform.localScale = s;
				deform.transform.localScale = new Vector3(s.y,s.y,1);
				boy.boy.transform.position = new Vector3(0,0,5);
			} else {
				boy.bg.material.mainTexture = bg;
			}
		}

		void Update() {
			if (deform == null && Input.GetMouseButtonDown(0)) {
				var p = cursor.tr.position;
				var b = boy.boy.bounds;
				if (p.x >= b.min.x && p.x <= b.max.x && p.y >= b.min.y && p.y <= b.max.y) {
					GameController.NextScene(Transition.Lens);
				}
			}
		}
	}
}