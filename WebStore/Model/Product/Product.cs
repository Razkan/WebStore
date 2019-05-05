namespace WebStore.Model.Product
{
    public interface ICategorized
    {
        Category Category { get; set; }
    }

    public interface Product : Identifiable, ICategorized
    {
    }
}
