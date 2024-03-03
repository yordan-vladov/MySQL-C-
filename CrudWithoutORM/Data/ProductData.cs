using MySql.Data.MySqlClient;

public class ProductData
{
    public List<Product> GetAll()
    {
        var productList = new List<Product>();
        using (var connection = Database.GetConnection())
        {
            var command = new MySqlCommand("SELECT * FROM Products", connection);
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var product = new Product(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetDecimal(2),
                        reader.GetInt32(3)
                    );

                    productList.Add(product);
                }

            }
            connection.Close();
        }

        return productList;
    }

    public Product? Get(int id)
    {
        Product? product = null;
        using (var connection = Database.GetConnection())
        {
            var command = new MySqlCommand("SELECT * FROM Products WHERE Id=@id", connection);
            command.Parameters.AddWithValue("id", id);
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    product = new Product(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetDecimal(2),
                            reader.GetInt32(3)
                    );
                }
            }

            connection.Close();
        }

        return product;
    }

    public void Add(Product product)
    {
        using (var connection = Database.GetConnection())
        {
            var command = new MySqlCommand("INSERT INTO Products (Name, Price, Stock) VALUES(@name, @price, @stock)", connection);
            command.Parameters.AddWithValue("name", product.Name);
            command.Parameters.AddWithValue("price", product.Price);
            command.Parameters.AddWithValue("stock", product.Stock);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    public void Update(Product product)
    {
        using (var connection = Database.GetConnection())
        {
            var command = new MySqlCommand("UPDATE Products SET Name=@name, Price=@price, Stock=@stock WHERE Id=@id", connection);
            command.Parameters.AddWithValue("id", product.Id);
            command.Parameters.AddWithValue("name", product.Name);
            command.Parameters.AddWithValue("price", product.Price);
            command.Parameters.AddWithValue("stock", product.Stock);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

    }

    public void Delete(int id)
    {
        using (var connection = Database.GetConnection())
        {
            var command = new MySqlCommand("DELETE Products WHERE Id=@id", connection);
            command.Parameters.AddWithValue("id", id);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

        }
    }
}