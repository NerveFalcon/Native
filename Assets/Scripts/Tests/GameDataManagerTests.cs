using System.Collections;
using GameData;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameDataManagerTests
{
    private GameDataManager _manager;

    [SetUp]
    public void Setup()
    {
        _manager = new("TestDatabase.db");
    }

    [TearDown]
    public void Teardown()
    {
        _manager.Dispose();
    }

    [Test]
    public void SaveAndLoad_ShouldWorkCorrectly()
    {
        // Arrange
        var testData = new TestData { Value = "Test" };

        // Act
        _manager.Save(testData);
        var loadedData = _manager.Load<TestData>();

        // Assert
        Assert.AreEqual(testData.Value, loadedData.Value);
    }

    [UnityTest]
    public IEnumerator SaveAsyncAndLoadAsync_ShouldWorkCorrectly()
    {
        // Arrange
        var testData = new TestData { Value = "AsyncTest" };

        // Act
        yield return _manager.SaveAsync(testData).AsIEnumerator();
        var loadedData = _manager.LoadAsync<TestData>();
        yield return loadedData.AsIEnumerator();

        // Assert
        Assert.AreEqual(testData.Value, loadedData.Result.Value);
    }

    [Test]
    public void Flush_ShouldPersistData()
    {
        // Arrange
        var testData = new TestData { Value = "FlushTest" };
        _manager.Save(testData);

        // Act
        _manager.Flush();

        // Create a new manager to ensure we're loading from disk
        _manager.Dispose();
        var newManager = new GameDataManager("TestDatabase.db");
        var loadedData = newManager.Load<TestData>();

        // Assert
        Assert.AreEqual(testData.Value, loadedData.Value);

        _manager = newManager;
    }

    private class TestData
    {
        public string Value { get; set; }
    }
}

public static class TaskExtensions
{
    public static IEnumerator AsIEnumerator(this System.Threading.Tasks.Task task)
    {
        while (!task.IsCompleted)
        {
            yield return null;
        }

        if (task.IsFaulted)
        {
            throw task.Exception;
        }
    }

    public static IEnumerator AsIEnumerator<T>(this System.Threading.Tasks.Task<T> task)
    {
        while (!task.IsCompleted)
        {
            yield return null;
        }

        if (task.IsFaulted)
        {
            throw task.Exception;
        }

        yield return task.Result;
    }
}