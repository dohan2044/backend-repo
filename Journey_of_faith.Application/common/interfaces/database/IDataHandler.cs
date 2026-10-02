namespace Journey_of_faith.Application.common.interfaces;


public interface IDataHandler
{
    Task<IEnumerable<T>> GetAllEntityAsync<T>(string talbe);
    Task<T?> GetEntityDetailsAsync<T>(int Id, string StoredProcedure);

    Task<bool> InsertOnlyName(string table, object param);
    Task<int> GetCountDataTable(string tableName);
}