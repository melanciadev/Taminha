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
		public string dialogueString;
		public bool hasChoice = false;
		public Choice dialogueChoice;
		public bool isAfterChoice = false;
		public bool isGoodPath = false;
	}

	[System.Serializable]
	public class Dialogue
	{
		public List<DialogueItem> dialogueList;
		public Character speaker;
	}
}