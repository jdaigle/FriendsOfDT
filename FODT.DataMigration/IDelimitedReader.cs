using System;

namespace FODT.DataMigration
{
    /// <summary>
    ///  Reads a row of characters from the current stream and returns the data.
    /// </summary>
    /// <returns>The next row from the input stream, or null if the end of the input stream is reached.</returns>
    public interface IDelimitedReader : IDisposable
    {
        IDelimitedRow NextRow();
        bool ReuseRow { get; }
    }
}
