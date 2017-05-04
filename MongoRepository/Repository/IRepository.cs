namespace RecommendIT.Common.Data.MongoRepository.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using MongoDB.Driver;

    /// <summary>
    /// An <c>interface</c> for common MongoDB data operations.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Gets the underling <see cref="IMongoCollection{TDocument}"/> used for queries.
        /// </summary>
        /// <value>
        /// The underling <see cref="IMongoCollection{TEntity}"/>.
        /// </value>
        IMongoCollection<TEntity> Collection { get; set;}

        #region Queries

        IMongoCollection<TEntity> GetCollection();

        /// <summary>
        /// Get the entity with the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the entity to get.</param>
        /// <returns>An instance of TEnity that has the specified identifier if found, otherwise null.</returns>
        TEntity Get(string key);

        /// <summary>
        /// Get the entity with the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the entity to get.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An instance of TEnity that has the specified identifier if found, otherwise null.</returns>
        Task<TEntity> GetAsync(string key, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get all entities as an <see cref="IQueryable{TEntity}"/>.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> of entities.</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Get all entities async.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<List<TEntity>> GetAllAsync( CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get the first entity using the specified <paramref name="criteria"/> expression.
        /// </summary>
        /// <param name="criteria">The criteria expression.</param>
        /// <returns>
        /// An instance of TEnity that matches the criteria if found, otherwise null.
        /// </returns>
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> criteria);

        /// <summary>
        /// Get the first entity using the specified <paramref name="criteria"/> expression.
        /// </summary>
        /// <param name="criteria">The criteria expression.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An instance of TEnity that matches the criteria if found, otherwise null.
        /// </returns>
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get all entities using the specified <paramref name="criteria"/> expression.
        /// </summary>
        /// <param name="criteria">The criteria expression.</param>
        /// <returns></returns>
        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> criteria);

        /// <summary>
        /// Get all entities using the specified <paramref name="criteria"/> expression.
        /// </summary>
        /// <param name="criteria">The criteria expression.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Determines if the specified <paramref name="criteria" /> exists.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>
        ///   <c>true</c> if criteria expression is found; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">criteria</exception>
        bool Any(Expression<Func<TEntity, bool>> criteria);

        #endregion

        #region Insert

        /// <summary>
        /// Inserts the specified <paramref name="entity"/> to the underlying data repository.
        /// </summary>
        /// <param name="entity">The entity to be inserted.</param>
        /// <returns>The entity that was inserted.</returns>
        TEntity Insert(TEntity entity);

        /// <summary>
        /// Inserts the specified <paramref name="entities"/> to the underlying data repository.
        /// </summary>
        /// <param name="entities">The entities to be inserted.</param>
        void Insert(IEnumerable<TEntity> entities);

        /// <summary>
        /// Inserts the specified <paramref name="entities"/> in a batch operation to the underlying data repository.
        /// </summary>
        /// <param name="entities">The entities to be inserted.</param>
        void InsertBatch(IEnumerable<TEntity> entities);


        /// <summary>
        /// Inserts the specified <paramref name="entity" /> to the underlying data repository.
        /// </summary>
        /// <param name="entity">The entity to be inserted.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The entity that was inserted.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">entity</exception>
        Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));
        #endregion

        #region Update
        /// <summary>
        /// Updates the specified <paramref name="entity"/> in the underlying data repository.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        /// <returns>The entity that was updated.</returns>
        TEntity Update(TEntity entity);

        /// <summary>
        /// Updates the specified <paramref name="entities"/> in the underlying data repository.
        /// </summary>
        /// <param name="entities">The entities to be updated.</param>
        void Update(IEnumerable<TEntity> entities);

        /// <summary>
        /// Updates the specified <paramref name="entity" /> in the underlying data repository.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The entity that was updated.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">entity</exception>
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));
        
        #endregion

        #region Delete

        /// <summary>
        /// Deletes an entity with the specified <paramref name="key"/> from the underlying data repository.
        /// </summary>
        /// <param name="key">The key of the entity to delete.</param>
        /// <returns>The number of documents deleted</returns>
        bool Delete(string key);

        /// <summary>
        /// Deletes the specified <paramref name="entity"/> from the underlying data repository.
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        /// <returns>The number of documents deleted</returns>
        bool Delete(TEntity entity);

        /// <summary>
        /// Deletes an entity with the specified <paramref name="id" /> from the underlying data repository.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of documents deleted</returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is <see langword="null" />.</exception>
        Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes the specified <paramref name="entity" /> from the underlying data repository.
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of documents deleted</returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <see langword="null" />.</exception>
        Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        #endregion
    }
}