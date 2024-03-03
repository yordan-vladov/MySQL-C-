using MySql.Data.MySqlClient;

public static class Database
{
    private static string connectionString = @"server=localhost;userid=yordan;password=1234;database=shop";
    public static MySqlConnection GetConnection()
    {
        return new MySqlConnection(connectionString);
    }
}