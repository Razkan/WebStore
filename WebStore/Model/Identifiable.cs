using WebStore.Db.Attribute;

namespace WebStore.Model
{
    /// <summary>
    /// Unique identification
    /// </summary>
    public interface Identifiable
    {
        [PrimaryKey]
        string Id { get; }
    }
}