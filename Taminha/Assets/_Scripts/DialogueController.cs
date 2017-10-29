using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
		public Text dialogueText;
		public bool isLastDialogue = false;

		[Header("Choices Made")]
		public bool isAfterChoice = false;
		public bool isGoodPath = false;

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

			//When a choice is being made...
			else if(currentStatus == DialogueStatus.Choosing)
			{
				//TODO -- CRAZY MOUSE


				//TESTE -TEMP
				if (Input.GetKeyUp (KeyCode.G)) {
					isGoodPath = true;
					isAfterChoice = true;
					currentStatus = DialogueStatus.None;
					ShowCurrentDialogue(currentDialogue.dialogueList);
				}

				if (Input.GetKeyUp (KeyCode.B)) {
					isGoodPath = false;
					isAfterChoice = true;
					currentStatus = DialogueStatus.None;
					ShowCurrentDialogue(currentDialogue.dialogueList);
				}

				//TESTE -TEMP
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
			if (!isLastDialogue) {
				//Reset all
				ResetDialogueTexts ();

				//Check if it is happening after a choice. 
				if (isAfterChoice) {
					//Check if it is a good one
					if (isGoodPath) {
						//And if the next one is, play it
						if (dialogueList [currentDialogueItem].isGoodPath) {
							ShowDialogueItem (dialogueList [currentDialogueItem]);
						} else {
							currentDialogueItem++;
							if (currentDialogueItem == dialogueList.Count)
								isLastDialogue = true;
							ShowCurrentDialogue (dialogueList);
						}
					}

					//Check if it is a bad one
					else {
						//And if the next one is, play it
						if (!dialogueList [currentDialogueItem].isGoodPath) {
							ShowDialogueItem (dialogueList [currentDialogueItem]);
						} else {
							currentDialogueItem++;
							if (currentDialogueItem == dialogueList.Count)
								isLastDialogue = true;
							ShowCurrentDialogue (dialogueList);
						}
					}
				} 

				//If it is not happening after a choice
				else {
					ShowDialogueItem (dialogueList [currentDialogueItem]);

					currentDialogueItem++;
					if (currentDialogueItem == dialogueList.Count)
						isLastDialogue = true;
				 
				}
			}

			else {
				print("FIM");
			}
		}
	
		//Call the right function depending if it`s a Regular or Choice one
		public void ShowDialogueItem(DialogueItem dialogueItem)
		{
			if (dialogueItem.hasChoice)
				StartCoroutine(StartChoiceDialogue(dialogueItem.dialogueString, dialogueItem.dialogueChoice));
			else
				StartCoroutine(StartRegularDialogue(dialogueItem.dialogueString));
		}

		//For the Regular Dialogues
		public IEnumerator StartRegularDialogue(string dialogue)
		{
			currentStatus = DialogueStatus.Showing;
			currentTextSpeed = regularTextSpeed;

			int currentChar = 0;
			string newString = "";
			dialogueText.text = newString;

			while (dialogue.Length - 1 >= currentChar)
			{
				newString += dialogue[currentChar];
				dialogueText.text = newString;
				yield return new WaitForSeconds(currentTextSpeed);

				//Wait twice as long when a Comma is printed.
				if (dialogue [currentChar] == ',') {
					yield return new WaitForSeconds(currentTextSpeed);
				}
				else if (dialogue [currentChar] == '!' || dialogue [currentChar] == '?') {
					yield return new WaitForSeconds(currentTextSpeed*2);
				}


				currentChar++;
			}

			currentStatus = DialogueStatus.Waiting;
			yield return null;
		}

		//For the choices Dialogues
		public IEnumerator StartChoiceDialogue(string dialogue, Choice choiceOption)
		{
			#region Show
			//It is Showing
			currentStatus = DialogueStatus.Showing;

			//Show the Dialogue
			var currentChar = 0;
			var newString = "";
			currentTextSpeed = regularTextSpeed;
			dialogueText.text = newString;
			while (dialogue.Length - 1 >= currentChar)
			{
				newString += dialogue[currentChar];
				dialogueText.text = newString;
				yield return new WaitForSeconds(currentTextSpeed);

				//Wait twice as long when a Comma is printed.
				if (dialogue [currentChar] == ',') {
					yield return new WaitForSeconds(currentTextSpeed);
				}
				else if (dialogue [currentChar] == '!' || dialogue [currentChar] == '?') {
					yield return new WaitForSeconds(currentTextSpeed*2);
				}

				currentChar++;
			}
			#endregion

			#region Choose
			//It is Choosing
			currentStatus = DialogueStatus.Choosing;

			//Show the choice Image
			dialogueChoiceDownImage.rectTransform.DOScale(1, 0.3f);

			//Show the Good Choice
			currentChar = 0;
			newString = "";
			dialogueGoodChoiceButtonText.text = newString;
			while (choiceOption.goodChoice.Length - 1 >= currentChar)
			{
				newString += choiceOption.goodChoice[currentChar];
				dialogueGoodChoiceButtonText.text = newString;
				yield return new WaitForSeconds(currentTextSpeed);

				currentChar++;
			}

			//Show the Bad Choice
			currentChar = 0;
			newString = "";
			dialogueBadChoiceButtonText.text = newString;
			while (choiceOption.badChoice.Length - 1 >= currentChar)
			{
				newString += choiceOption.badChoice[currentChar];
				dialogueBadChoiceButtonText.text = newString;
				yield return new WaitForSeconds(currentTextSpeed);

				currentChar++;
			}
			#endregion
		}
			
		//Reset all content from the dialogues boxes
		public void ResetDialogueTexts()
		{
			dialogueText.text = "";
			dialogueGoodChoiceButtonText.text = "";
			dialogueBadChoiceButtonText.text = "";

			if(dialogueChoiceDownImage.rectTransform.localScale.x != 0)
			dialogueChoiceDownImage.rectTransform.DOScale(0, 0.3f);
		}
	}
}