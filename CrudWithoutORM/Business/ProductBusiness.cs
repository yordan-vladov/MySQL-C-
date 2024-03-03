public class ProductBusiness
{
    private ProductData manager = new ProductData();

    public List<Product> GetAll()
    {
        return manager.GetAll();
    }

    public Product Get(int id)
    {
        return manager.Get(id) ?? new Product();
    }

    public void Add(Product product)
    {
        manager.Add(product);
    }

    public void Update(Product product)
    {
        manager.Update(product);
    }

    public void Delete(int id)
    {
        manager.Delete(id);
    }
}