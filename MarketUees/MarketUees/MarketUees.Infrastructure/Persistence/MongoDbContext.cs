using MarketUees.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace MarketUees.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        static MongoDbContext()
        {
            RegisterClassMaps();
        }

        public MongoDbContext(IMongoDatabase database)
        {
            _database = database;
        }

        // Mapea Id (string) → _id (ObjectId) sin tocar el Domain
        private static void RegisterClassMaps()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(BaseEntity)))
            {
                BsonClassMap.RegisterClassMap<BaseEntity>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdProperty(e => e.Id)
                      .SetIdGenerator(StringObjectIdGenerator.Instance)
                      .SetSerializer(new StringSerializer(BsonType.ObjectId));
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Producto)))
            {
                BsonClassMap.RegisterClassMap<Producto>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Compra)))
            {
                BsonClassMap.RegisterClassMap<Compra>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Resena)))
            {
                BsonClassMap.RegisterClassMap<Resena>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }
        }

        public IMongoCollection<Producto> Productos =>
            _database.GetCollection<Producto>("productos");

        public IMongoCollection<Compra> Compras =>
            _database.GetCollection<Compra>("compras");

        public IMongoCollection<Resena> Resenas =>
            _database.GetCollection<Resena>("resenas");
    }
}
