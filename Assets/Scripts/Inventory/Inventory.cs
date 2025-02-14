using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
	[Serializable]
	public class Inventory
	{
		public List<Item> items = new List<Item>();
		public event Action OnItemChangedCallback;

		public void AddItem(Item newItem)
		{
			// Проверяем, есть ли уже такой предмет в инвентаре
			var existingItem = items.Find(item => item.name == newItem.name);
			if (existingItem != null)
			{
				// Если предмет уже есть, увеличиваем его количество
				existingItem.count++;
			}
			else
			{
				// Если предмета нет, добавляем его в инвентарь
				items.Add(newItem);
			}
			OnItemChangedCallback?.Invoke();
		}

		public void RemoveItem(Item itemToRemove)
		{
			var existingItem = items.Find(item => item.name == itemToRemove.name);
			if (existingItem == null) return;
			
			existingItem.count--;
			if (existingItem.count <= 0)
			{
				items.Remove(existingItem);
			}
			OnItemChangedCallback?.Invoke();
		}

		public void DisplayInventory()
		{
			foreach (var item in items)
			{
				Debug.Log($"Item: {item.name}, Count: {item.count}");
			}
		}
	}
}