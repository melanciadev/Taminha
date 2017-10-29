using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Taminha {
	public class DeformBoyController:MonoBehaviour {
		public MeshDeformerController mesh;
		public TextMesh timerMesh;
		public TextMesh scoreMesh;

		float transition = 0;
		bool changedScene = false;
		bool done = false;

		void Update() {
			timerMesh.text = Mathf.CeilToInt(mesh.timer).ToString();
			if (mesh.done) {
				if (!done) {
					done = true;
					scoreMesh.text = Mathf.RoundToInt(GameController.deformScore*100)+"%";
				}
				transition += Time.deltaTime;
				if (!changedScene && transition >= 2) {
					changedScene = true;
					GameController.NextScene(Transition.Lens);
				}
				var t = Mathf.Clamp01(1-transition*3);
				scoreMesh.transform.localScale = Vector3.one*(1+t*t*.25f);
			}
		}
	}
}