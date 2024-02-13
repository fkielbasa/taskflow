namespace TaskFlow.Models
{

    public class DatabaseSettings : IDatabaseSettings
    {
        public string UsersCollectionName { get; set; } = String.Empty;
        public string ConnectionString { get; set; } = String.Empty;
        public string DatabaseName { get; set; } = String.Empty;
    }
}
