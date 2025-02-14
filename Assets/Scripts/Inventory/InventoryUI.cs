using UnityEngine;

namespace Inventory
{
	public class InventoryUI : MonoBehaviour
	{
		public Inventory inventory;
		public Transform itemsParent;
		public GameObject inventoryUI;

		InventorySlot[] slots;

		void Start()
		{
			inventory.OnItemChangedCallback += UpdateUI;
			slots = itemsParent.GetComponentsInChildren<InventorySlot>();
			UpdateUI();
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.I))
			{
				inventoryUI.SetActive(!inventoryUI.activeSelf);
			}
		}

		void UpdateUI()
		{
			for (var i = 0; i < slots.Length; i++)
			{
				if (i < inventory.items.Count)
				{
					slots[i].AddItem(inventory.items[i]);
				}
				else
				{
					slots[i].ClearSlot();
				}
			}
		}
	}
}