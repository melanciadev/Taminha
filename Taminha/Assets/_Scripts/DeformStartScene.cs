using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Taminha {
	public class DeformStartScene:MonoBehaviour {
		public MeshDeformerController mesh;
		public Camera cam;
		public Renderer lockRend;
		public Texture2D[] lockTex;

		float transition = 0;
		bool done = false;

		void Update() {
			if (mesh.done) {
				if (!done) {
					done = true;
					GameController.NextScene(Transition.Fade,2,.5f);
				}
				transition += Time.deltaTime*.5f;
				float t = transition*transition;
				cam.orthographicSize = Mathf.Lerp(5,1,t);
				cam.transform.position = Vector2.Lerp(Vector2.zero,lockRend.transform.position,t);
			}
			lockRend.material.mainTexture = lockTex[(int)(Time.time*12)%lockTex.Length];
		}
	}
}