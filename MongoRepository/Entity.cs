namespace RecommendIT.Common.Data.MongoRepository
{
    using System;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Bson.Serialization.IdGenerators;

    /// <summary>
    /// A base <c>class</c> for a MongoDB Entity
    /// </summary>
    [BsonIgnoreExtraElements(true, Inherited = true)]
    public class Entity : IEntity
    {
        public Entity()
        {
            this.UniqueId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets or sets the date the entity was created.
        /// </summary>
        /// <value>
        /// The date the entity was created.
        /// </value>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the entity.
        /// </summary>
        /// <value>
        /// The identifier for the entity.
        /// </value>
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the date the entity was updated.
        /// </summary>
        /// <value>
        /// The date the entity was updated.
        /// </value>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ModifiedOn { get; set; }

        public string UniqueId { get; set; }

        public bool IsDeleted { get; set; }
    }
}