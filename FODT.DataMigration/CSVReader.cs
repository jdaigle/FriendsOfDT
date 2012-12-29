using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FODT.DataMigration
{
    public class CSVReader : IDisposable, IDelimitedReader, IEnumerable<IDelimitedRow>
    {
        private StreamReader reader;
        private bool reuseRow;
        private CSVRow row;

        public CSVReader(Stream stream, bool reuseRow)
        {
            this.reader = new StreamReader(stream);
            this.reuseRow = reuseRow;
        }

        public CSVReader(Stream stream)
            : this(stream, false) { }

        public bool ReuseRow { get { return reuseRow; } }

        /// <summary>
        ///  Reads a row of characters from the current stream and returns the data.
        /// </summary>
        /// <returns>The next row from the input stream, or null if the end of the input stream is reached.</returns>
        public IDelimitedRow NextRow()
        {
            var line = reader.ReadLine();
            if (line == null) return null;

            if (reuseRow)
            {
                if (row == null) row = new CSVRow();
                row.ClearFields();
            }
            else
                row = new CSVRow();

            int fieldStartingPosition = 0;
            bool doubledQuotedField = false;
            bool inDoubledQuotedField = false;
            for (int i = 0; i < line.Length; i++)
            {
                if (i == fieldStartingPosition)
                {
                    // detect if field is double quoted
                    if (line[i] == '"')
                    {
                        doubledQuotedField = true;
                        inDoubledQuotedField = true;
                    }
                    else
                    {
                        doubledQuotedField = false;
                        inDoubledQuotedField = false;
                    }
                }

                if (i != fieldStartingPosition && line[i] == '"')
                {
                    // ignore if end of the line
                    if (i != line.Length - 1)
                    {
                        // is this quote escaped?
                        if (line[i + 1] == '"')
                        {
                            // skip next char
                            i++;
                            continue;
                        }
                        else
                        {
                            // end of double quoted field most likely
                            inDoubledQuotedField = false;
                        }
                    }
                }

                if (line[i] == ',' && !inDoubledQuotedField)
                {
                    var length = i - fieldStartingPosition;
                    row.AddField(line.Substring(fieldStartingPosition, length), doubledQuotedField);
                    fieldStartingPosition = i + 1; // start at the next position in the string

                    //end of string
                    if (i == line.Length - 1)
                    {
                        // after last comma is empty
                        row.AddField(string.Empty, doubledQuotedField);
                    }
                }

                //end of string
                if (i == line.Length - 1 && fieldStartingPosition <= i)
                {
                    // remainder of the string
                    row.AddField(line.Substring(fieldStartingPosition), doubledQuotedField);
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            var row = NextRow();
            while (row != null)
            {
                yield return row;
                row = NextRow();
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
    }

    public class CSVRow : IEnumerable<string>, IDelimitedRow
    {
        private IList<string> fieldData = new List<string>();

        public void AddField(string data, bool doubleQuotedField)
        {
            if (doubleQuotedField)
            {
                // Remove quote at beginning and end of data
                data = data.Substring(1, data.Length - 2);
                // Find instances of "" and replace with "
                data = data.Replace("\"\"", "\"");
            }
            fieldData.Add(data);
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
