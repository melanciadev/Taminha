using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Taminha {
	public class AudioController:MonoBehaviour {
		static AudioController me;

		AudioSource aud;

		void Awake() {
			me = this;
			aud = gameObject.AddComponent<AudioSource>();
		}

		public static void Play(AudioClip clip,float volume = 1) {
			me.aud.PlayOneShot(clip,volume);
		}
	}
}