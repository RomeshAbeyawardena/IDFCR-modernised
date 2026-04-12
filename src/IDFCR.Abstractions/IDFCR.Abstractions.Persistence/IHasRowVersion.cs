namespace IDFCR.Abstractions.Persistence
{
    /// <summary>
    /// Defines an interface for entities that include a row version for concurrency control. Repositories can check the row version before performing updates or deletes to ensure that the record has not been modified by another process since it was retrieved.
    /// </summary>
    public interface IHasRowVersion
    {
        /// <summary>
        /// Gets or sets the row version for concurrency control. This is typically a byte array that is updated by the database on each modification of the record. Repositories can use this value to detect concurrent modifications and prevent data conflicts.
        /// </summary>
        byte[] RowVersion { get; set; }
    }
}
