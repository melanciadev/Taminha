using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Taminha {
	public class DialogueCursor:MonoBehaviour {
		public GameObject cursorPrefab;
		public DialogueController dialogue;

		QuadCursor cursor;

		void Awake() {
			cursor = Instantiate(cursorPrefab).GetComponent<QuadCursor>();
			cursor.show = false;
			cursor.cam = GetComponent<Camera>();
			cursor.tr.parent = transform;
			cursor.tr.localPosition = new Vector3(0,0,cursor.tr.localPosition.z);
			cursor.SetDialogue();
		}
		
		void Start() {
			//
		}

		void LateUpdate() {
			if (dialogue.currentStatus == DialogueStatus.Choosing) {
				if (!cursor.show) {
					cursor.show = true;
					cursor.tr.localPosition = new Vector3(.25f,-4,cursor.tr.localPosition.z);
				}
				if (cursor.tr.localPosition.x > 1.75f) {
					dialogue.inputSelectSide = 1;
				} else if (cursor.tr.localPosition.x < -1.25f) {
					dialogue.inputSelectSide = -1;
				} else {
					dialogue.inputSelectSide = 0;
				}
				cursor.transparent = dialogue.inputSelectSide != 0;
			} else {
				cursor.show = false;
				dialogue.inputSelectSide = 0;
			}
		}
	}
}