using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Taminha
{
	[System.Serializable]
	public enum DialogueStatus
	{
		None,
		Showing,
		Waiting,
		Choosing,
		End
	}

	[System.Serializable]
	public enum Gamestate
	{
		OpenScreen,
		MainScreen,
		GameScreen
	}

	[System.Serializable]
	public enum Character
	{
		ChitchatGirl,
		CoolProfessor,
		TennisClubProfessor
	}

}