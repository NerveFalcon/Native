using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Inventory
{
	public class InventorySlot : MonoBehaviour
	{
		public Image icon;
		public TextMeshProUGUI countText;

		private Item _item;

		public void AddItem(Item newItem)
		{
			_item = newItem;

			icon.sprite = _item.icon;
			icon.enabled = true;
			countText.text = _item.count > 1 ? _item.count.ToString() : "";
			countText.enabled = _item.count > 1;
		}

		public void ClearSlot()
		{
			_item = null;

			icon.sprite = null;
			countText.text = "(-)";
		}
	}
}