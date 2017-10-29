using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Taminha {
	public class DeformBoyController:MonoBehaviour {
		public MeshDeformerController mesh;
		public TextMesh timerMesh;

		float transition = 0;
		bool changedScene = false;

		void Update() {
			timerMesh.text = Mathf.CeilToInt(mesh.timer).ToString();
			if (mesh.done) {
				transition += Time.deltaTime;
				if (!changedScene && transition >= 2) {
					changedScene = true;
					GameController.NextScene(Transition.Lens);
				}
			}
		}
	}
}