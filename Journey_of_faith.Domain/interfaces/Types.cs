namespace Journey_of_faith.Domain.interfaces;



public interface IGetOneToOneData<TData, TEntity> where TData: notnull
{
    Task<Dictionary<TData, TEntity>> GetOneToOneDataAsync(string sql, IReadOnlyList<TData> ids, Func<TEntity, TData> keySelector, CancellationToken cancellationToken = default);
}




public interface IGetOneToManyData<TData, TEntity> where TData : notnull
{
    Task<Dictionary<TData, TEntity[]>> GetDataByIdsAsync(string sql, IReadOnlyList<TData> ids, Func<TEntity, TData> keySelector, CancellationToken cancellationToken = default);
}



// public interface IMappingBatchDataLoader<TData, TEntity> where TData: notnull where TEntity : class
// {
//     Task<Dictionary<TData, TEntity>> LoadBatchAsync(IReadOnlyList<TData> keys, CancellationToken cancellationToken);
// }
