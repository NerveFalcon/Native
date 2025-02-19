using System;
using UnityEngine;

namespace GameData
{
	public class GameDataProxy : MonoBehaviour
	{
		private GameDataManager gameDataManager;
		private void Awake()
		{
			gameDataManager = GameDataManagerSingleton.Instance;
		}
	}
	public static class GameDataManagerSingleton
	{
		public static GameDataManager Instance { get; }
		
		static GameDataManagerSingleton()
		{
			Instance = new();
		}
	}

}