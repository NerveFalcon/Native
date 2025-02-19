using System;
using UnityEngine;

namespace GameData
{
	public class GameDataProxy : MonoBehaviour
	{
		private GameDataManager gameDataManager;
		private void Awake()
		{
			gameDataManager = GameDataManager.Instance;
		}
	}
}