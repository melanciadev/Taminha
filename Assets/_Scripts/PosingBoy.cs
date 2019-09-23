using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Taminha {
	public class PosingBoy:MonoBehaviour {
		public Renderer bg;
		public Renderer boy;
		public Texture2D[] tex;
		
		void Start() {
			//
		}

		void Update() {
			boy.material.mainTexture = tex[(int)(Time.time*12)%tex.Length];
		}
	}
}