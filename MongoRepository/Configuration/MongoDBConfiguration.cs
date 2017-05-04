namespace RecommendIT.Common.Data.MongoRepository.Configuration
{
    public class MongoDbConfiguration
    {
        public string ConnectionString { get; set; }

        public string Database { get; set; }

        public string CollectionName { get; set; }
    }
}