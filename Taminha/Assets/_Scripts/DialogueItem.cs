using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Taminha
{
	[System.Serializable]
	public class Choice
	{
		public string goodChoice;
		public string badChoice;
	}

	[System.Serializable]
	public class DialogueItem
	{
		public bool isGhostTalk = false;
		public string dialogueString;
		public bool hasChoice = false;
		public Choice dialogueChoice;
	}

	[System.Serializable]
	public class Dialogue
	{
		public List<DialogueItem> dialogueList;
		public Character speaker;
	}
}