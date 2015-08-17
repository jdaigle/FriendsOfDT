using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FODT.DataMigration
{
    public class TSVReader : IDisposable, IDelimitedReader, IEnumerable<IDelimitedRow>
    {
        private StreamReader reader;
        private bool reuseRow;
        private TSVRow row;

        public TSVReader(Stream stream, bool reuseRow)
        {
            this.reader = new StreamReader(stream);
            this.reuseRow = reuseRow;
        }

        public TSVReader(Stream stream)
            : this(stream, false) { }

        public bool ReuseRow { get { return reuseRow; } }

        /// <summary>
        ///  Reads a row of characters from the current stream and returns the data.
        /// </summary>
        /// <returns>The next row from the input stream, or null if the end of the input stream is reached.</returns>
        public IDelimitedRow NextRow()
        {
            if (disposed) return null;
            var line = reader.ReadLine();
            if (line == null) return null;

            if (reuseRow)
            {
                if (row == null) row = new TSVRow();
                row.ClearFields();
            }
            else
                row = new TSVRow();

            int fieldStartingPosition = 0;
            for (int i = 0; i < line.Length; i++)
            {

                if (line[i] == '\t')
                {
                    var length = i - fieldStartingPosition;
                    row.AddField(line.Substring(fieldStartingPosition, length));
                    fieldStartingPosition = i + 1; // start at the next position in the string
                    //end of string
                    if (i == line.Length - 1)
                    {
                        // after last tab is empty
                        row.AddField(string.Empty);
                    }
                }

                //end of string
                if (i == line.Length - 1 && fieldStartingPosition <= i)
                {
                    // remainder of the string
                    row.AddField(line.Substring(fieldStartingPosition));
                }
            }

            return row;
        }

        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (disposed)
                    return;
                if (this.reader != null)
                {
                    this.reader.Dispose();
                }
                disposed = true;
            }
        }

        public IEnumerator<IDelimitedRow> GetEnumerator()
        {
            var row = NextRow();
            while (row != null)
            {
                yield return row;
                row = NextRow();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var row = NextRow();
            while (row != null)
            {
                yield return row;
                row = NextRow();
            }
        }
    }

    public class TSVRow : IEnumerable<string>, IDelimitedRow
    {
        private IList<string> fieldData = new List<string>();

        public void AddField(string data)
        {
            fieldData.Add(data.Replace("\\N", ""));
        }

        public void ClearFields()
        {
            fieldData.Clear();
        }

        public int Length { get { return fieldData.Count; } }

        public string this[int index]
        {
            get { return fieldData[index]; }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return (IEnumerator<string>)fieldData;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)fieldData;
        }
    }
}
