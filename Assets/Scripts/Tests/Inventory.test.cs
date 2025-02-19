using NUnit.Framework;
using System.Collections.Generic;
using Inventory;

namespace Tests
{
	public class InventoryTests
	{
		private Inventory.Inventory inventory;

		[SetUp]
		public void Setup()
		{
			inventory = new Inventory.Inventory();
		}

		[Test]
		public void AddItem_EmptyInventory_AddsNewItem()
		{
			// Arrange
			var newItem = new Item { name = "TestItem", count = 1 };

			// Act
			inventory.AddItem(newItem);

			// Assert
			Assert.AreEqual(1, inventory.items.Count);
			Assert.AreEqual(newItem.name, inventory.items[0].name);
			Assert.AreEqual(newItem.count, inventory.items[0].count);
		}

		[Test]
		public void AddItem_ExistingItem_IncreasesCount()
		{
			// Arrange
			var existingItem = new Item { name = "TestItem", count = 1 };
			inventory.AddItem(existingItem);
			var newItem = new Item { name = "TestItem", count = 2 };

			// Act
			inventory.AddItem(newItem);

			// Assert
			Assert.AreEqual(1, inventory.items.Count);
			Assert.AreEqual(existingItem.name, inventory.items[0].name);
			Assert.AreEqual(3, inventory.items[0].count);
		}

		[Test]
		public void RemoveItem_CountReachesZero_RemovesItemCompletely()
		{
			// Arrange
			var itemToRemove = new Item { name = "TestItem", count = 2 };
			inventory.AddItem(itemToRemove);

			// Act
			inventory.RemoveItem(itemToRemove);

			// Assert
			Assert.AreEqual(0, inventory.items.Count);
			Assert.IsFalse(inventory.items.Exists(item => item.name == "TestItem"));
		}

		[Test]
		public void RemoveItem_PartialRemoval_DecreasesCount()
		{
			// Arrange
			var existingItem = new Item { name = "TestItem", count = 5 };
			inventory.AddItem(existingItem);
			var itemToRemove = new Item { name = "TestItem", count = 2 };

			// Act
			inventory.RemoveItem(itemToRemove);

			// Assert
			Assert.AreEqual(1, inventory.items.Count);
			Assert.AreEqual(existingItem.name, inventory.items[0].name);
			Assert.AreEqual(3, inventory.items[0].count);
		}

		[Test]
		public void RemoveItem_NonExistentItem_DoesNotChangeInventory()
		{
			// Arrange
			var existingItem = new Item { name = "ExistingItem", count = 1 };
			inventory.AddItem(existingItem);
			var nonExistentItem = new Item { name = "NonExistentItem", count = 1 };

			// Act
			inventory.RemoveItem(nonExistentItem);

			// Assert
			Assert.AreEqual(1, inventory.items.Count);
			Assert.AreEqual(existingItem.name, inventory.items[0].name);
			Assert.AreEqual(existingItem.count, inventory.items[0].count);
		}

		[Test]
		public void AddItem_MultipleDifferentItems_AddsAllItems()
		{
			// Arrange
			var item1 = new Item { name = "Item1", count = 2 };
			var item2 = new Item { name = "Item2", count = 3 };
			var item3 = new Item { name = "Item3", count = 1 };

			// Act
			inventory.AddItem(item1);
			inventory.AddItem(item2);
			inventory.AddItem(item3);

			// Assert
			Assert.AreEqual(3, inventory.items.Count);
			Assert.IsTrue(inventory.items.Exists(item => item.name == "Item1" && item.count == 2));
			Assert.IsTrue(inventory.items.Exists(item => item.name == "Item2" && item.count == 3));
			Assert.IsTrue(inventory.items.Exists(item => item.name == "Item3" && item.count == 1));
		}

		[Test]
		public void RemoveItem_RemovingMoreThanAvailable_DoesNotModifyInventory()
		{
			// Arrange
			var existingItem = new Item { name = "TestItem", count = 3 };
			inventory.AddItem(existingItem);
			var itemToRemove = new Item { name = "TestItem", count = 5 };

			// Act
			inventory.RemoveItem(itemToRemove);

			// Assert
			Assert.AreEqual(1, inventory.items.Count);
			Assert.AreEqual(existingItem.name, inventory.items[0].name);
			Assert.AreEqual(3, inventory.items[0].count);
		}
	}
}