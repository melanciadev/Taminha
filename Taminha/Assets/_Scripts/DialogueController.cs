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

		[Header("Dialogue background")]
		public Image backgroundImage;
		public Sprite[] characterBackgrounds;

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
		public Text dialogueGoodChoiceButtonText;
		public Text dialogueBadChoiceButtonText;

		[Header("Input")]
		public int inputSelectSide = 0;

		[Header("Animation")]
		public Image boyImage;
		public Vector2 startBoySize;
		public Vector2 finalBoySize;
		public Image speakerImage;
		public float startSpeakerPosition;
		public float finalSpeakerPosition;
		public Image balloonImage;
		public Vector2 startBalloonScale;
		public Vector2 finalBalloonScale;
		public float speedOutGood = 0.5f;
		public float speedOutBad = 0.1f;

		[Header("Audio")]
		public AudioSource audioSourceBgm;
		public AudioSource audioSourceSpeech;
		public AudioClip dialogueBgm;
		public AudioClip struggleBgm;
		public AudioClip winSfx;
		public AudioClip failSfx;
		public AudioClip[] speechSfx;
		int audioState = 0;

		public void Start()
		{
			//Change to the right Balloon, speaker
			int speaker = (int)currentDialogue.speaker;
			topBalloonAnimator.SetInteger("ballonIndex", speaker);
			speakerAnimator.SetInteger("characterIndex", speaker);
			backgroundImage.sprite = characterBackgrounds[speaker];
			audioSourceSpeech.clip = speechSfx[speaker];
			audioSourceSpeech.volume = 0;
			audioSourceSpeech.Play();

			EnterAnimation();
			audioSourceBgm.clip = dialogueBgm;
			audioSourceBgm.Play();
		}
		
		public void EnterAnimation()
		{
			//Start positions
			boyImage.rectTransform.DOSizeDelta(startBoySize, 0.0f);
			speakerImage.rectTransform.DOAnchorPosX(startSpeakerPosition, 0.0f);
			balloonImage.rectTransform.DOScale(startBalloonScale, 0.0f);

			//Final Positions
			boyImage.rectTransform.DOSizeDelta(finalBoySize, 1.5f).OnComplete(() =>
				{
					speakerImage.rectTransform.DOAnchorPosX(finalSpeakerPosition, 0.5f).OnComplete(() =>
						{
							balloonImage.rectTransform.DOScale(finalBalloonScale, 0.5f).OnComplete(() =>
								{
									ShowAct(currentDialogue);
								});
						});
				});
		}

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
				if (inputSelectSide != 0 && Input.GetMouseButtonUp(0)) {
					isGoodPath = inputSelectSide < 0;
					GameController.Play(isGoodPath ? failSfx : winSfx,.2f);
					GameController.goodChars += isGoodPath ? 1 : 0;
					isAfterChoice = true;
					currentStatus = DialogueStatus.None;
					ShowCurrentDialogue(currentDialogue.dialogueList);
				}
			}
		}
			
		public void ExitAnimation()
		{
			balloonImage.rectTransform.DOScale (startBalloonScale, 0.2f).OnComplete(() =>
				{
					if(isGoodPath)
					{
						speakerImage.rectTransform.DOAnchorPosX(startSpeakerPosition, speedOutGood).OnComplete(LeaveScene);
					}
					else
					{
						speakerImage.rectTransform.DOAnchorPosX(startSpeakerPosition, speedOutBad).OnComplete(LeaveScene);
					}
				});
		}

		void LeaveScene() {
			if (currentDialogue.showLaterScreen) {
				GameController.NextScene(Transition.Later);
			} else {
				GameController.NextScene(Transition.Fade,1,2);
			}
		}

		//Show all the act
		public void ShowAct(Dialogue currentDialogue)
		{
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
						if (dialogueList [currentDialogueItem].isGoodPath)
						{
							ShowDialogueItem (dialogueList [currentDialogueItem]);
							currentDialogueItem++;
							if (currentDialogueItem + 1 == dialogueList.Count)
							{
								isLastDialogue = true;
							}
						} 
						else
						{
							currentDialogueItem++;
							if (currentDialogueItem + 1 >= dialogueList.Count)
							{
								isLastDialogue = true;
							}
							ShowCurrentDialogue(dialogueList);
						}
					}

					//Check if it is a bad one
					else {
						//And if the next one is, play it
						if (!dialogueList[currentDialogueItem].isGoodPath)
						{
							ShowDialogueItem (dialogueList [currentDialogueItem]);
							currentDialogueItem++;
							if (currentDialogueItem + 1 == dialogueList.Count)
							{
								isLastDialogue = true;
							}
						}
						else
						{
							currentDialogueItem++;
							if (currentDialogueItem + 1 >= dialogueList.Count)
							{
								isLastDialogue = true;
							}
							ShowCurrentDialogue(dialogueList);
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
				currentStatus = DialogueStatus.End;
				ExitAnimation ();
			}
		}
	
		//Call the right function depending if it`s a Regular or Choice one
		public void ShowDialogueItem(DialogueItem dialogueItem)
		{
			audioSourceSpeech.volume = 0;
			if (dialogueItem.hasChoice) {
				StartCoroutine(StartChoiceDialogue(dialogueItem.dialogueString,dialogueItem.dialogueChoice));
				if (audioState == 0) {
					audioState = 1;
					audioSourceBgm.clip = struggleBgm;
					audioSourceBgm.time = 0;
					audioSourceBgm.Play();
				}
			} else {
				StartCoroutine(StartRegularDialogue(dialogueItem.dialogueString));
				if (audioState == 1) {
					audioState = 2;
					audioSourceBgm.Stop();
				}
			}
		}

		//For the Regular Dialogues
		public IEnumerator StartRegularDialogue(string dialogue)
		{
			audioSourceSpeech.volume = 1;
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
			audioSourceSpeech.volume = 0;
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