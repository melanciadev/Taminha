using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Melancia.Taminha
{
	[System.Serializable]
	public enum DialogueStatus
	{
		None,
		Showing,
		Waiting,
		Choosing
	}

	public class DialogueController : MonoBehaviour
	{
		[Header("Text Speed")]
		public float slowTextSpeed = 0.2f;
		public float regularTextSpeed = 0.05f;
		public float fastTextSpeed = 0.01f;
		private float currentTextSpeed;

		[Header("Dialogue Collection")]
		public List<DialogueItem> dialogueList;
		public List<DialogueItem> currentDialogList;

		[Header("Dialogue")]
		public int currentDialogueItem;
		public DialogueStatus currentStatus = DialogueStatus.None;

		[Header("Dialogue regular text")]
		public Text dialogueUpText;
		public Text dialogueDownText;

		[Header("Dialogue choice text")]
		public Text dialogueChoiceDownText;
		public Image dialogueChoiceDownImage;
		public Text dialogueGoodChoiceButtonText; //TEMP
		public Text dialogueBadChoiceButtonText; //TEMP


		//TESTE
		public void Start()
		{
			currentDialogList = dialogueList;
			currentDialogueItem = 0;
			ShowAllDialogue(currentDialogList, currentDialogueItem);
		}
		//TESTE



		//Check click to speedup text
		public void Update()
		{
			if (currentStatus == DialogueStatus.Showing)
			{
				if (Input.GetMouseButtonDown(0)) 
					currentTextSpeed = regularTextSpeed;
				if (Input.GetMouseButtonUp(0))
					currentTextSpeed = fastTextSpeed;
			}

			else if (currentStatus == DialogueStatus.Waiting)
			{
				if (Input.GetMouseButtonUp (0))
				{
					ShowAllDialogue(currentDialogList, currentDialogueItem);
				}
			}
		}

		//Show all the Dialogue group (Act)
		public void ShowAllDialogue(List<DialogueItem> dialogueList, int currentItem)
		{
			ResetDialogueTexts();
			if (currentDialogueItem <= dialogueList.Count)
				ShowDialogueItem(dialogueList[currentDialogueItem]);
			currentDialogueItem++;

			//TODO - What happens when the last dialog is made?
		}
	
		//Call the right function depending if it`s a Regular or choice one
		public void ShowDialogueItem(DialogueItem dialogueItem)
		{
			if (dialogueItem.hasChoice)
				StartCoroutine(StartChoiceDialogue(dialogueItem.dialogueChoice));
			else
				StartCoroutine(StartRegularDialogue(dialogueItem.isGhostTalk, dialogueItem.dialogueString));
		}

		//For the Regular Dialogues
		public IEnumerator StartRegularDialogue(bool isGhostTalk, string dialogue)
		{
			currentStatus = DialogueStatus.Showing;
			currentTextSpeed = regularTextSpeed;

			int currentChar = 0;
			string newString = "";
			Text currentText = isGhostTalk ? dialogueDownText : dialogueUpText;
			currentText.text = newString;

			while (dialogue.Length - 1 >= currentChar)
			{
				newString += dialogue[currentChar];
				currentText.text = newString;
				yield return new WaitForSeconds(currentTextSpeed);
				currentChar++;
			}

			currentStatus = DialogueStatus.Waiting;
			yield return null;
		}

		//For the choices Dialogues
		public IEnumerator StartChoiceDialogue(Choice choiceOption)
		{
			currentStatus = DialogueStatus.Choosing;

			dialogueChoiceDownImage.gameObject.SetActive(true);
			dialogueGoodChoiceButtonText.text = choiceOption.goodChoice;
			dialogueBadChoiceButtonText.text = choiceOption.badChoice;

			yield return null;

			//TODO - ESCOLHA DA RESPOSTA
		}

		//Reset all content from the dialogues boxes
		public void ResetDialogueTexts()
		{
			dialogueUpText.text = "";
			dialogueDownText.text = "";
			dialogueChoiceDownText.text = "";
			dialogueGoodChoiceButtonText.text = "";
			dialogueBadChoiceButtonText.text = "";
			dialogueChoiceDownImage.gameObject.SetActive(false);
		}
	}
}