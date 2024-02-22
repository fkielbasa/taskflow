using MongoDB.Driver;

namespace TaskFlow.Models
{
    public class MongoDbContext
    {
        private IMongoDatabase _database;
        private readonly IDatabaseSettings _settings;

        public MongoDbContext(IDatabaseSettings settings)
        {
            _settings = settings;
            CreateTTLIndexOnExpiryDate();
        }

        public IMongoCollection<PasswordResetToken> PasswordResetTokens
        {
            get
            {
                return _database.GetCollection<PasswordResetToken>(_settings.ResetTokensCollectionName);
            }
        }

        private void CreateTTLIndexOnExpiryDate()
        {
            var collection = _database.GetCollection<PasswordResetToken>(_settings.ResetTokensCollectionName);
            var options = new CreateIndexOptions { ExpireAfter = TimeSpan.Zero };
            var indexModel = new CreateIndexModel<PasswordResetToken>(
                Builders<PasswordResetToken>.IndexKeys.Ascending(t => t.ExpiryDate), options);

            collection.Indexes.CreateOne(indexModel);
        }
    }
}
