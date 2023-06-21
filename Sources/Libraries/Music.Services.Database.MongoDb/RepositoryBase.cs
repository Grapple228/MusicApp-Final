using System.Linq.Expressions;
using MongoDB.Driver;
using Music.Services.Database.Common.Repositories;
using Music.Shared.Common;

namespace Music.Services.Database.MongoDb;

public class RepositoryBase<TModel> : IRepository<TModel> where TModel : IModel
{
    private readonly IMongoCollection<TModel> _dbCollection;
    private readonly FilterDefinitionBuilder<TModel> _filterBuilder = Builders<TModel>.Filter;

    public RepositoryBase(IMongoDatabase database, string collectionName)
    {
        _dbCollection = database.GetCollection<TModel>(collectionName);
    }

    public async Task<long> GetItemsCountAsync() => 
        await _dbCollection.CountDocumentsAsync(_filterBuilder.Empty);
    public async Task<long> GetItemsCountAsync(Expression<Func<TModel, bool>> filter) => 
        await _dbCollection.CountDocumentsAsync(filter);
    
    private async Task<long> GetPagesCountAsync(long countPerPage)
    {
        var totalCount = await GetItemsCountAsync();
        var pagesCount = totalCount % countPerPage == 0
            ? totalCount / countPerPage
            : totalCount / countPerPage + 1;
        return pagesCount == 0 ? 1 : pagesCount;
    }
    private async Task<long> GetPagesCountAsync(long countPerPage, Expression<Func<TModel, bool>> filter)
    {
        var totalCount = await GetItemsCountAsync(filter);
        var pagesCount = totalCount % countPerPage == 0
            ? totalCount / countPerPage
            : totalCount / countPerPage + 1;
        return pagesCount == 0 ? 1 : pagesCount;
    }
    
    public async Task<PageResult<TModel>> GetAllFromPageAsync(long pageNumber, long countPerPage)
    {
        if (pageNumber < 1) throw new Exception("Page number can't be less than 1!");
        var totalCount = await GetItemsCountAsync();
        var pagesCount = await GetPagesCountAsync(countPerPage);
        if (pageNumber > pagesCount) 
            pageNumber = pagesCount;
        var itemsToSkipCount = countPerPage * (pageNumber == 0 ? 1 : pageNumber - 1);
        var itemsOnPage = await _dbCollection.Find(_filterBuilder.Empty).SortBy(x => x.Id).Skip((int)itemsToSkipCount).Limit((int)countPerPage).ToListAsync();
        return new PageResult<TModel>(pagesCount, pageNumber, totalCount, itemsOnPage);
    }
    
    public async Task<PageResult<TModel>> GetAllFromPageAsync(long pageNumber, long countPerPage,Expression<Func<TModel, bool>> filter)
    {
        if (pageNumber < 1) throw new Exception("Page number can't be less than 1!");
        var totalCount = await GetItemsCountAsync(filter);
        var pagesCount = await GetPagesCountAsync(countPerPage, filter);
        if (pageNumber > pagesCount) 
            pageNumber = pagesCount;
        var itemsToSkipCount = countPerPage * (pageNumber == 0 ? 1 : pageNumber - 1);
        var itemsOnPage = await _dbCollection.Find(filter).SortBy(x => x.Id).Skip((int)itemsToSkipCount).Limit((int)countPerPage).ToListAsync();
        return new PageResult<TModel>(pagesCount, pageNumber, totalCount, itemsOnPage);
    }

    public async Task<IReadOnlyCollection<TModel>> GetAllAsync() =>
        await _dbCollection.Find(_filterBuilder.Empty).SortBy(x => x.Id).ToListAsync();

    public async Task<IReadOnlyCollection<TModel>> GetAllAsync(Expression<Func<TModel, bool>> filter) =>
        await _dbCollection.Find(filter).SortBy(x => x.Id).ToListAsync();

    public async Task<IReadOnlyCollection<TModel>> GetAllAsync(IEnumerable<Guid> ids) =>
        await _dbCollection.Find(x => ids.Contains(x.Id)).SortBy(x => x.Id).ToListAsync();

    public async Task<TModel?> GetAsync(Guid id)
    {
        var filter = _filterBuilder.Eq(entity => entity.Id, id);
        return await _dbCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<TModel?> GetAsync(Expression<Func<TModel, bool>> filter) =>
        await _dbCollection.Find(filter).FirstOrDefaultAsync();

    public async Task CreateAsync(TModel entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        await _dbCollection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(TModel entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        var filter = _filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
        await _dbCollection.ReplaceOneAsync(filter, entity);
    }

    public async Task RemoveAsync(Guid id)
    {
        var filter = _filterBuilder.Eq(entity => entity.Id, id);
        await _dbCollection.DeleteOneAsync(filter);
    }
}