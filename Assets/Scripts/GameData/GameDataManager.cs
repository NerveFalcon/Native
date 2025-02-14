using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using UnityEngine;

public class GameDataManager : IDisposable
{
    private LiteDatabase _db;
    private readonly string _dbFile;
    private readonly ConcurrentDictionary<Type, object> Cache = new();
    private readonly ConcurrentDictionary<Type, bool> DirtyFlags = new();
    private readonly ReaderWriterLockSlim _rwLock = new();
    private string DbPath => Path.Combine(Application.persistentDataPath, _dbFile ?? "GameData.db");

    public GameDataManager(string dbFile) : this() => _dbFile = dbFile;
    public GameDataManager() => InitializeDatabase();

    private void InitializeDatabase()
    {
        
        try
        {
            _db = new(DbPath);
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка инициализации базы данных: {e.Message}");
        }
    }

    public Task<T> LoadAsync<T>(CancellationToken cancellationToken = default) where T : new()
        => Task.Run(() => Load<T>(), cancellationToken);

    public Task SaveAsync<T>(T data, CancellationToken cancellationToken = default)
        => Task.Run(() => Save(data), cancellationToken);

    public Task FlushAsync(IProgress<int> progress = null, CancellationToken cancellationToken = default)
        => Task.Run(() => Flush(progress), cancellationToken);

    public T Load<T>() where T : new()
    {
        var type = typeof(T);

        _rwLock.EnterReadLock();
        try
        {
            if (Cache.TryGetValue(type, out var cachedData))
            {
                return (T)cachedData;
            }
        }
        finally
        {
            _rwLock.ExitReadLock();
        }

        _rwLock.EnterWriteLock();
        try
        {
            if (_db == null) return new();

            var collection = _db.GetCollection<T>(type.Name);
            var data = collection.FindOne(Query.All()) ?? new T();
            Cache[type] = data;
            return data;
        }
        finally
        {
            _rwLock.ExitWriteLock();
        }
    }

    public void Flush(IProgress<int> progress = null)
    {
        _rwLock.EnterWriteLock(); // Блокируем запись
        try
        {
            var dirtyTypes = DirtyFlags.Keys.Where(t => DirtyFlags[t]).ToList();
            DirtyFlags.Clear(); // Сбрасываем флаги атомарно

            int totalTypes = dirtyTypes.Count;
            int completedTypes = 0;

            try
            {
                _db.BeginTrans();

                foreach (var type in dirtyTypes)
                {
                    var collection = _db.GetCollection(type.Name);
                    collection.DeleteAll();

                    var bson = _db.Mapper.ToDocument(Cache[type]);
                    collection.Insert(bson);

                    completedTypes++;
                    progress?.Report((completedTypes * 100) / totalTypes);
                }

                _db.Commit();
            }
            catch (Exception e)
            {
                _db.Rollback();

                foreach (var type in dirtyTypes)
                {
                    DirtyFlags[type] = true;
                }

                Debug.LogError($"Ошибка сохранения: {e.Message}");
                progress?.Report(-1);
            }
        }
        finally
        {
            _rwLock.ExitWriteLock(); // Снимаем блокировку
        }
    }

    public void Save<T>(T data)
    {
        var type = typeof(T);

        _rwLock.EnterWriteLock();
        try
        {
            Cache[type] = data;
            DirtyFlags[type] = true;
        }
        finally
        {
            _rwLock.ExitWriteLock();
        }
    }

    public void Dispose()
    {
        _db?.Dispose();
        _rwLock?.Dispose();
    }
}