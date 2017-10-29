using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Melancia.Taminha
{
	public class DialogueController : MonoBehaviour
	{
		[Header("Text Speed")]
		public float slowTextSpeed = 0.2f;
		public float regularTextSpeed = 0.05f;
		public float fastTextSpeed = 0.01f;
		private float currentTextSpeed;

		[Header("Dialogue Collection")]
		public Dialogue currentDialogue;

		[Header("Dialogue animator")]
		public Animator topBalloonAnimator;
		public Animator speakerAnimator;

		[Header("Dialogue")]
		public int currentDialogueItem;
		public DialogueStatus currentStatus = DialogueStatus.None;

		[Header("Dialogue regular text")]
		public Text dialogueUpText;
		public Text dialogueDownText;

		[Header("Dialogue choice text")]
		public Image dialogueChoiceDownImage;
		public Text dialogueGoodChoiceButtonText; //TEMP
		public Text dialogueBadChoiceButtonText; //TEMP


		//TESTE -TEMP
		public void Start()
		{
			ShowAct(currentDialogue);
		}
		//TESTE -TEMP



		public void Update()
		{
			//When the text is being showed, speed it.
			if (currentStatus == DialogueStatus.Showing)
			{
				if (Input.GetMouseButtonDown(0)) 
					currentTextSpeed = regularTextSpeed;
				if (Input.GetMouseButtonUp(0))
					currentTextSpeed = fastTextSpeed;
			}

			//When the text end, click to go to the next 
			else if (currentStatus == DialogueStatus.Waiting)
			{
				if (Input.GetMouseButtonUp (0))
				{
					ShowCurrentDialogue(currentDialogue.dialogueList);
				}
			}
		}
			
		//Show all the act
		public void ShowAct(Dialogue currentDialogue)
		{
			//Change to the right Balloon and speaker
			switch (currentDialogue.speaker) {
			case Character.ChitchatGirl:
				topBalloonAnimator.SetInteger("ballonIndex", 0);
				speakerAnimator.SetInteger("characterIndex", 0);
				break;
			case Character.CoolProfessor:
				topBalloonAnimator.SetInteger("ballonIndex", 1);
				speakerAnimator.SetInteger("characterIndex", 1);
				break;
			case Character.TennisClubProfessor:
				topBalloonAnimator.SetInteger("ballonIndex", 2);
				speakerAnimator.SetInteger("characterIndex", 2);
				break;
			}


			currentDialogueItem = 0;
			ShowCurrentDialogue (currentDialogue.dialogueList);
		}

		//Show the current dialogue
		public void ShowCurrentDialogue(List<DialogueItem> dialogueList)
		{
			ResetDialogueTexts();

			if (currentDialogueItem < dialogueList.Count)
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

				//Wait twice as long when a Comma is printed.
				if (dialogue [currentChar] == ',') {
					yield return new WaitForSeconds(currentTextSpeed);
				}

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
			dialogueGoodChoiceButtonText.text = "";
			dialogueBadChoiceButtonText.text = "";
			dialogueChoiceDownImage.gameObject.SetActive(false);
		}
	}
}