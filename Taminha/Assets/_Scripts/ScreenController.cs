using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Melancia.Taminha {


	public class ScreenController : MonoBehaviour {
		public Gamestate currentGamestate = Gamestate.OpenScreen;

		[Header("Logo Images")]
		public Image spjamLogoImage;
		public Image melanciaLogoImage;

		[Header("Main Screen")]
		public Image mainScreenImage;
		//TODO - Title Image and other elements


		public static ScreenController instance;
		public void Awake()
		{
			instance = this;
		}


		public void Start()
		{
			//Start Opening Sequence
			Sequence startSequence = DOTween.Sequence();
			startSequence.Append(spjamLogoImage.DOFade(1, 2f));
			startSequence.Append(spjamLogoImage.DOFade(0, 1f));
			startSequence.Append(melanciaLogoImage.DOFade(1, 2f));
			startSequence.Append(melanciaLogoImage.DOFade(0, 1f));
			startSequence.Append(mainScreenImage.DOFade(1, 2f));
			startSequence.OnComplete(()=> {
				currentGamestate = Gamestate.MainScreen;
			});

			//TODO: Aplicar Sons: Som de Fundo, Som de Menu, Som de Deformar Fantasminha
		}

		void Update () 
		{
			//Only if on the main screen
			if(currentGamestate == Gamestate.MainScreen)
			{
				print("Start the Game!!");
				//TODO: Fazer FANTASMA entrar na fechadura e realizar transição para primeira tela de jogo.
			}
		}
	}
}
