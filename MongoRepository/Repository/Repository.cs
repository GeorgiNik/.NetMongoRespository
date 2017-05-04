namespace RecommendIT.Common.Data.MongoRepository.Repository
{
    
    using Microsoft.Extensions.Configuration;
    using MongoDB.Driver;
    using Polly;
    using Polly.Retry;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using RecommendIT.Common.Data.MongoRepository.Configuration;
    using RecommendIT.Common.Data.MongoRepository.Helpers;

    /// <summary>
    /// A MongoDB data repository base class.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        public Repository(IOptions<MongoDbConfiguration> settings)
        {
            this.Collection = CollectionHelpers<TEntity>.GetCollectionFromConnectionString(settings.Value.ConnectionString);
        }

        public IMongoCollection<TEntity> Collection { get; set; }

        #region Queries

        public IMongoCollection<TEntity> GetCollection()
        {
            return this.Collection;
        }

        /// <summary>
        /// Finds the entity with the specified identifier.
        /// </summary>
        /// <param name="key">The entity identifier.</param>
        /// <returns>The entity with the specified identifier.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null" />.</exception>
        public TEntity Get(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            
            return Retry(()=>
            {
                this.Collection.Find(this.KeyExpression(key)).FirstOrDefault();
            });
        }

        /// <summary>
        /// Finds the entity with the specified identifier.
        /// </summary>
        /// <param name="key">The entity identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The entity with the specified identifier.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null" />.</exception>
        public Task<TEntity> GetAsync(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return this.Collection.Find(this.KeyExpression(key)).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Get all <typeparamref name="TEntity" /> entities as an IQueryable
        /// </summary>
        /// <returns>
        /// IQueryable of <typeparamref name="TEntity" />.
        /// </returns>
        public IQueryable<TEntity> GetAll()
        {
            return this.Collection.AsQueryable();
        }

        /// <summary>
        /// Get all entities async
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">criteria</exception>
        public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.Collection.Find(x => true).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Find the first entity using the specified <paramref name="criteria" /> expression.
        /// </summary>
        /// <param name="criteria">The criteria expression.</param>
        /// <returns>
        /// An instance of TEnity that matches the criteria if found, otherwise null.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">criteria</exception>
        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            return this.Collection.Find(criteria).FirstOrDefault();
        }

        /// <summary>
        /// Find the first entity using the specified <paramref name="criteria" /> expression.
        /// </summary>
        /// <param name="criteria">The criteria expression.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An instance of TEnity that matches the criteria if found, otherwise null.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">criteria</exception>
        public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            return this.Collection.Find(criteria).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Find all entities using the specified <paramref name="criteria" /> expression.
        /// </summary>
        /// <param name="criteria">The criteria expression.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">criteria</exception>
        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            return this.Collection.AsQueryable().Where(criteria);
        }

        /// <summary>
        /// Find all entities using the specified <paramref name="criteria" /> expression.
        /// </summary>
        /// <param name="criteria">The criteria expression.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">criteria</exception>
        public Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.Collection.Find(criteria).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Determines if the specified <paramref name="criteria" /> exists.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>
        ///   <c>true</c> if criteria expression is found; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">criteria</exception>
        public bool Any(Expression<Func<TEntity, bool>> criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            return this.Collection.AsQueryable().Any(criteria);
        }

        #endregion

        #region CRUD

        #region Insert

        /// <summary>
        /// Inserts the specified <paramref name="entity" /> to the underlying data repository.
        /// </summary>
        /// <param name="entity">The entity to be inserted.</param>
        /// <returns>
        /// The entity that was inserted.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">entity</exception>
        public TEntity Insert(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            this.BeforeInsert(entity);
            this.Collection.InsertOne(entity);

            return entity;
        }

        /// <summary>
        /// Inserts the specified <paramref name="entities" /> to the underlying data repository.
        /// </summary>
        /// <param name="entities"></param>
        /// <exception cref="System.ArgumentNullException">entities</exception>
        public void Insert(IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            foreach (TEntity entity in entities)
            {
                this.Insert(entity);
            }
        }

        /// <summary>
        /// Inserts the specified <paramref name="entities" /> in a batch operation to the underlying data repository.
        /// </summary>
        /// <param name="entities">The entities to be inserted.</param>
        /// <exception cref="System.ArgumentNullException">entities</exception>
        public void InsertBatch(IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            List<TEntity> list = entities.ToList();
            list.ForEach(this.BeforeInsert);

            this.Collection.InsertMany(list);
        }

        /// <summary>
        /// Inserts the specified <paramref name="entity" /> to the underlying data repository.
        /// </summary>
        /// <param name="entity">The entity to be inserted.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The entity that was inserted.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">entity</exception>
        public Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            this.BeforeInsert(entity);

            return this.Collection.InsertOneAsync(entity, cancellationToken: cancellationToken).ContinueWith(t => entity, cancellationToken);
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates the specified <paramref name="entity" /> in the underlying data repository.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        /// <returns>
        /// The entity that was updated.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">entity</exception>
        public TEntity Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            this.BeforeUpdate(entity);

            var updateOptions = new UpdateOptions { IsUpsert = true };
            string key = entity.Id;

            this.Collection.ReplaceOne(this.KeyExpression(key), entity, updateOptions);

            return entity;
        }

        /// <summary>
        /// Updates the specified <paramref name="entities" /> in the underlying data repository.
        /// </summary>
        /// <param name="entities"></param>
        /// <exception cref="System.ArgumentNullException">entities</exception>
        public void Update(IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            foreach (TEntity entity in entities)
            {
                this.Update(entity);
            }
        }

        /// <summary>
        /// Updates the specified <paramref name="entity" /> in the underlying data repository.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The entity that was updated.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">entity</exception>
        public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            this.BeforeUpdate(entity);

            var updateOptions = new UpdateOptions { IsUpsert = true };
            string key = entity.Id;

            return this.Collection
                .ReplaceOneAsync(this.KeyExpression(key), entity, updateOptions, cancellationToken)
                .ContinueWith(t => entity, cancellationToken);
        }

        #endregion

        #region Save

        /// <summary>
        /// Saves the specified <paramref name="entity" /> in the underlying data repository by inserting if doesn't exist, or updating if it does.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        /// <returns>
        /// The entity that was updated.
        /// </returns>
        public TEntity Save(TEntity entity)
        {
            return this.Update(entity);
        }

        /// <summary>
        /// Saves the specified <paramref name="entities" /> in the underlying data repository by inserting if doesn't exist, or updating if it does.
        /// </summary>
        /// <param name="entities"></param>
        public void Save(IEnumerable<TEntity> entities)
        {
            this.Update(entities);
        }

        /// <summary>
        /// Saves the specified <paramref name="entity" /> in the underlying data repository by inserting if doesn't exist, or updating if it does.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The entity that was updated.
        /// </returns>
        public Task<TEntity> SaveAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.UpdateAsync(entity, cancellationToken);
        }

        #endregion

        #region Delete
        /// <summary>
        /// Deletes the specified <paramref name="entity" /> from the underlying data repository.
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of documents deleted</returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <see langword="null" />.</exception>
        public Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            string key = entity.Id;
            return this.DeleteAsync(key, cancellationToken);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The number of documents deleted</returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is <see langword="null" />.</exception>
        public bool Delete(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            DeleteResult result = this.Collection.DeleteOne(this.KeyExpression(id));
            return result.IsAcknowledged;
        }

        /// <summary>
        /// Deletes the specified <paramref name="entity" /> from the underlying data repository.
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        /// <returns>The number of documents deleted</returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <see langword="null" />.</exception>
        public bool Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            string key = entity.Id;
            return this.Delete(key);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of documents deleted</returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is <see langword="null" />.</exception>
        public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return this.Collection
                .DeleteOneAsync(this.KeyExpression(id), cancellationToken)
                .ContinueWith(t => t.Result.IsAcknowledged, cancellationToken);
        }

#endregion

        #endregion

        #region Entity

        /// <summary>
        /// Called before an insert.
        /// </summary>
        /// <param name="entity">The entity.</param>
        private void BeforeInsert(TEntity entity)
        {
            entity.CreatedOn = DateTime.Now;
            entity.ModifiedOn = DateTime.Now;
        }

        /// <summary>
        /// Called before an update.
        /// </summary>
        /// <param name="entity">The entity.</param>
        private void BeforeUpdate(TEntity entity)
        {
            if (entity.CreatedOn == DateTime.MinValue)
            {
                entity.CreatedOn = DateTime.Now;
            }

            entity.ModifiedOn = DateTime.Now;
        }

        /// <summary>
        /// Gets the key expression with the specified <paramref name="key" />.
        /// </summary>
        /// <param name="key">The key to get expression with.</param>
        /// <returns>
        /// The key expression for the specified key.
        /// </returns>
        /// <example>
        ///   <code>
        /// Example xpression for an entity key.
        /// <![CDATA[entity => entity.Id == key]]></code>
        /// </example>
        protected Expression<Func<TEntity, bool>> KeyExpression(string key)
        {
            return entity => entity.Id == key;
        }

        #endregion

        #region RetryPolicy
        /// <summary>
        /// retry operation for three times if IOException occurs
        /// </summary>
        /// <typeparam name="TResult">return type</typeparam>
        /// <param name="action">action</param>
        /// <returns>action result</returns>
        /// <example>
        /// return Retry(() => 
        /// { 
        ///     do_something;
        ///     return something;
        /// });
        /// </example>
        protected virtual TResult Retry<TResult>(Func<TResult> action)
        {
            return Policy
                .Handle<MongoConnectionException>(i => i.InnerException.GetType() == typeof(IOException) || i.InnerException.GetType() == typeof(SocketException))
                .Retry(3)
                .Execute(action);
        }
        #endregion
    }
}