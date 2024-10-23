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
    private static readonly Dictionary<Tuple<string, string>, DataEntity> _databaseInstance =
        new Dictionary<Tuple<string, string>, DataEntity>();

    public InMemoryDatabase()
    {
    }

    public bool Insert(T data)
    {
        try
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
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Insert: Failed to add entity with SiteId: {data.SiteId}, CompanyCode: {data.CompanyCode}. Exception: {ex.Message}");
            return false;
        }
    }

    public bool Update(T data)
    {
        try
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
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Update: Failed to update entity with SiteId: {data.SiteId}, CompanyCode: {data.CompanyCode}. Exception: {ex.Message}");
            return false;
        }
    }

    public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
    {
        try
        {
            var entities = FindAll();
            var result = entities.Where(expression.Compile());
            Log.Information($"Find: Found {result.Count()} entities matching the criteria");
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Find: Failed to find entities. Exception: {ex.Message}");
            return Enumerable.Empty<T>();
        }
    }

    public IEnumerable<T> FindAll()
    {
        try
        {
            var result = _databaseInstance.Values.OfType<T>();
            Log.Information($"FindAll: Found {result.Count()} entities");
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"FindAll: Failed to retrieve entities. Exception: {ex.Message}");
            return Enumerable.Empty<T>();
        }
    }

    public bool Delete(Expression<Func<T, bool>> expression)
    {
        try
        {
            var entities = Find(expression);
            foreach (var dataEntity in entities)
            {
                _databaseInstance.Remove(Tuple.Create(dataEntity.SiteId, dataEntity.CompanyCode));
                Log.Information($"Delete: Removed entity with SiteId: {dataEntity.SiteId}, CompanyCode: {dataEntity.CompanyCode}");
            }
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Delete: Failed to delete entities. Exception: {ex.Message}");
            return false;
        }
    }

    public bool DeleteAll()
    {
        try
        {
            _databaseInstance.Clear();
            Log.Information("DeleteAll: Cleared all entities");
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"DeleteAll: Failed to clear entities. Exception: {ex.Message}");
            return false;
        }
    }

    public bool UpdateAll(Expression<Func<T, bool>> filter, string fieldToUpdate, object newValue)
    {
        try
        {
            var entities = Find(filter);
            foreach (var dataEntity in entities)
            {
                var newEntity = UpdateProperty(dataEntity, fieldToUpdate, newValue);
                _databaseInstance[Tuple.Create(dataEntity.SiteId, dataEntity.CompanyCode)] = newEntity;
                Log.Information($"UpdateAll: Updated entity with SiteId: {dataEntity.SiteId}, CompanyCode: {dataEntity.CompanyCode}");
            }
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"UpdateAll: Failed to update entities. Exception: {ex.Message}");
            return false;
        }
    }

    private T UpdateProperty(T dataEntity, string key, object value)
    {
        Type t = typeof(T);
        var prop = t.GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (prop == null)
        {
            throw new Exception("Property not found");
        }

        prop.SetValue(dataEntity, value, null);
        return dataEntity;
    }

    public async Task<bool> InsertAsync(T data)
    {
        return await Task.FromResult(Insert(data));
    }

    public async Task<bool> UpdateAsync(T data)
    {
        return await Task.FromResult(Update(data));
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
    {
        return await Task.FromResult(Find(expression));
    }

    public async Task<IEnumerable<T>> FindAllAsync()
    {
        return await Task.FromResult(FindAll());
    }

    public async Task<bool> DeleteAsync(Expression<Func<T, bool>> expression)
    {
        return await Task.FromResult(Delete(expression));
    }

    public async Task<bool> DeleteAllAsync()
    {
        return await Task.FromResult(DeleteAll());
    }

    public async Task<bool> UpdateAllAsync(Expression<Func<T, bool>> filter, string fieldToUpdate, object newValue)
    {
        return await Task.FromResult(UpdateAll(filter, fieldToUpdate, newValue));
    }
}
