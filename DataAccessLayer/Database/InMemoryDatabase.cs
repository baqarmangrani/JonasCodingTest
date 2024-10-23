using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

public class InMemoryDatabase<T> : IDbWrapper<T> where T : DataEntity
{
    private static readonly Dictionary<Tuple<string, string>, DataEntity> _databaseInstance = new Dictionary<Tuple<string, string>, DataEntity>();

    public bool Insert(T data)
    {
        return ExecuteDatabaseOperation(() =>
        {
            var key = Tuple.Create(data.SiteId, data.CompanyCode);
            if (_databaseInstance.ContainsKey(key))
            {
                Log.Error($"Insert: Entity with SiteId: {data.SiteId}, CompanyCode: {data.CompanyCode} already exists.");
                return false;
            }
            _databaseInstance.Add(key, data);
            Log.Information($"Insert: Added entity with SiteId: {data.SiteId}, CompanyCode: {data.CompanyCode}");
            return true;
        }, $"Insert: Failed to add entity with SiteId: {data.SiteId}, CompanyCode: {data.CompanyCode}");
    }

    public bool Update(T data)
    {
        return ExecuteDatabaseOperation(() =>
        {
            var key = Tuple.Create(data.SiteId, data.CompanyCode);
            if (_databaseInstance.ContainsKey(key))
            {
                _databaseInstance[key] = data;
                Log.Information($"Update: Updated entity with SiteId: {data.SiteId}, CompanyCode: {data.CompanyCode}");
                return true;
            }
            Log.Error($"Update: Entity with SiteId: {data.SiteId}, CompanyCode: {data.CompanyCode} not found");
            return false;
        }, $"Update: Failed to update entity with SiteId: {data.SiteId}, CompanyCode: {data.CompanyCode}");
    }

    public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
    {
        return ExecuteDatabaseOperation(() =>
        {
            var entities = FindAll();
            var result = entities.Where(expression.Compile());
            Log.Information($"Find: Found {result.Count()} entities matching the criteria");
            return result;
        }, "Find: Failed to find entities");
    }

    public IEnumerable<T> FindAll()
    {
        return ExecuteDatabaseOperation(() =>
        {
            var result = _databaseInstance.Values.OfType<T>();
            Log.Information($"FindAll: Found {result.Count()} entities");
            return result;
        }, "FindAll: Failed to retrieve entities");
    }

    public bool Delete(Expression<Func<T, bool>> expression)
    {
        return ExecuteDatabaseOperation(() =>
        {
            var entities = Find(expression);
            foreach (var dataEntity in entities)
            {
                _databaseInstance.Remove(Tuple.Create(dataEntity.SiteId, dataEntity.CompanyCode));
                Log.Information($"Delete: Removed entity with SiteId: {dataEntity.SiteId}, CompanyCode: {dataEntity.CompanyCode}");
            }
            return true;
        }, "Delete: Failed to delete entities");
    }

    public bool DeleteAll()
    {
        return ExecuteDatabaseOperation(() =>
        {
            _databaseInstance.Clear();
            Log.Information("DeleteAll: Cleared all entities");
            return true;
        }, "DeleteAll: Failed to clear entities");
    }

    public bool UpdateAll(Expression<Func<T, bool>> filter, string fieldToUpdate, object newValue)
    {
        return ExecuteDatabaseOperation(() =>
        {
            var entities = Find(filter);
            foreach (var dataEntity in entities)
            {
                var newEntity = UpdateProperty(dataEntity, fieldToUpdate, newValue);
                _databaseInstance[Tuple.Create(dataEntity.SiteId, dataEntity.CompanyCode)] = newEntity;
                Log.Information($"UpdateAll: Updated entity with SiteId: {dataEntity.SiteId}, CompanyCode: {dataEntity.CompanyCode}");
            }
            return true;
        }, "UpdateAll: Failed to update entities");
    }

    private T UpdateProperty(T dataEntity, string key, object value)
    {
        var prop = typeof(T).GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (prop == null)
        {
            throw new Exception("Property not found");
        }
        prop.SetValue(dataEntity, value, null);
        return dataEntity;
    }

    public Task<bool> InsertAsync(T data) => Task.FromResult(Insert(data));
    public Task<bool> UpdateAsync(T data) => Task.FromResult(Update(data));
    public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression) => Task.FromResult(Find(expression));
    public Task<IEnumerable<T>> FindAllAsync() => Task.FromResult(FindAll());
    public Task<bool> DeleteAsync(Expression<Func<T, bool>> expression) => Task.FromResult(Delete(expression));
    public Task<bool> DeleteAllAsync() => Task.FromResult(DeleteAll());
    public Task<bool> UpdateAllAsync(Expression<Func<T, bool>> filter, string fieldToUpdate, object newValue) => Task.FromResult(UpdateAll(filter, fieldToUpdate, newValue));

    private TResult ExecuteDatabaseOperation<TResult>(Func<TResult> operation, string errorMessage)
    {
        try
        {
            return operation();
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"{errorMessage}. Exception: {ex.Message}");
            return default;
        }
    }
}
