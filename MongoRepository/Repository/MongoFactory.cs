namespace RecommendIT.Common.Data.MongoRepository.Repository
{
    using MongoDB.Driver;

    /// <summary>
    /// A helper class for getting MongoDB database connection.
    /// </summary>
    public static class MongoFactory
    {
        /// <summary>
        /// Gets the <see cref="IMongoDatabase"/> with the specified connection string.
        /// </summary>
        /// <param name="connectionString">The MongoDB connection string.</param>
        /// <returns>An instance of <see cref="IMongoDatabase"/>.</returns>
        public static IMongoDatabase GetDatabaseFromConnectionString(string connectionString)
        {
            var mongoUrl = new MongoUrl(connectionString);
            return GetDatabaseFromMongoUrl(mongoUrl);
        }

        /// <summary>
        /// Gets the <see cref="IMongoDatabase" /> with the specified <see cref="MongoUrl" />.
        /// </summary>
        /// <param name="mongoUrl">The mongo URL.</param>
        /// <returns>
        /// An instance of <see cref="IMongoDatabase" />.
        /// </returns>
        public static IMongoDatabase GetDatabaseFromMongoUrl(MongoUrl mongoUrl)
        {
            var client = new MongoClient(mongoUrl);
            IMongoDatabase mongoDatabase = client.GetDatabase(mongoUrl.DatabaseName);
            return mongoDatabase;
        }
    }
}