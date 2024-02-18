namespace TaskFlow.Models
{
    public interface IDatabaseSettings
    {
        string UsersCollectionName { get; set; } 
        string ResetTokensCollectionName { get; set; } 
        string ConnectionString { get; set; } 
        string DatabaseName { get; set; } 
    }
}
