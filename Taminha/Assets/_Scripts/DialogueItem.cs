using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Taminha
{
	[System.Serializable]
	public class Choice
	{
		public bool isGoodChoice = true;
		public string choiceString;

		public Choice(bool isGoodChoice, string choiceString)
		{
			this.isGoodChoice = isGoodChoice;
			this.choiceString = choiceString;
		}
	}

	[System.Serializable]
	public class DialogueChoice
	{
		public List<Choice> choiceList;
	}

	[System.Serializable]
	public class DialogueItem
	{
		public bool isGhostTalk = false;
		public string dialogueString;
		public bool hasChoice = false;
		public DialogueChoice dialogueChoice;
	}
}