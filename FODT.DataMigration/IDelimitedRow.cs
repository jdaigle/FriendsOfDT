using System.Collections.Generic;

namespace FODT.DataMigration
{
    public interface IDelimitedRow : IEnumerable<string>
    {
        int Length { get; }
        string this[int index] { get; }
    }
}
