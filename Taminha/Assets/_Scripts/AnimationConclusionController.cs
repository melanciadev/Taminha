using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Taminha {
	public class AnimationConclusionController:MonoBehaviour {
		public SpriteRenderer goodSprite;
		public SpriteRenderer neutralSprite;
		public SpriteRenderer badSprite;
		public TextMesh textMesh;
		public Sprite[] goodTex;
		public Sprite[] badTex;
		float tempo = 0;
		bool done = false;
		
		void Start() {
			int c = GameController.goodChars;
			switch (c) {
				case 0:
					goodSprite.enabled = false;
					neutralSprite.enabled = false;
					badSprite.enabled = true;
					textMesh.text = "Você se esforçou para decepcionar.";
					break;
				case 3:
					goodSprite.enabled = true;
					neutralSprite.enabled = false;
					badSprite.enabled = false;
					textMesh.text = "Você se passou por mininu, sem esforço.\nParece que isso é natural para você.";
					break;
				default:
					goodSprite.enabled = false;
					neutralSprite.enabled = true;
					badSprite.enabled = false;
					textMesh.text = "Você nem sempre vai ter controle de tudo\ne deve estar preparado para isso.";
					break;
			}
		}

		void Update() {
			if (goodSprite.enabled) {
				goodSprite.sprite = goodTex[(int)(Time.time*12)%goodTex.Length];
			}
			if (badSprite.enabled) {
				badSprite.sprite = badTex[(int)(Time.time*12)%badTex.Length];
			}
			if (!done) {
				tempo += Time.deltaTime;
				if (tempo >= 10 || Input.GetMouseButtonUp(0)) {
					done = true;
					GameController.NextScene(Transition.Fade,2,1);
				}
			}
		}
	}
}