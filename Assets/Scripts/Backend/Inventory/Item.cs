using UnityEngine;

namespace Inventory
{
	[System.Serializable]
	public class Item
	{
		public string name;
		public Sprite icon;
		public int count = 1;

		public Item(string name, Sprite icon)
		{
			this.name = name;
			this.icon = icon;
		}

		public Item() { }
	}
}