using WebStore.Db.Attribute;

namespace WebStore.Model
{
    /// <summary>
    /// Unique identification
    /// </summary>
    public interface IDatabaseEntity
    {
        [PrimaryKey]
        string Id { get; }
    }
}