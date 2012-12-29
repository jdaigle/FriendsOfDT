using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace FODT.DataMigration
{
    public class DynamicDelimitedReader : IDisposable, IEnumerable
    {
        private IDelimitedReader reader;
        private bool initialized;
        private DynamicDelimitedRow dynamicRow;
        private IDictionary<string, int> headers;

        public DynamicDelimitedReader(IDelimitedReader reader)
        {
            this.reader = reader;
        }

        public void Initialize()
        {
            if (initialized)
            {
                return;
            }

            // expecting first row to be headers
            var headersRow = reader.NextRow();
            if (headersRow == null)
            {
                throw new InvalidOperationException("No Data In File: Cannot Read");
            }

            headers = new Dictionary<string, int>();
            for (int i = 0; i < headersRow.Length; i++)
            {
                headers.Add(headersRow[i].Trim().Replace(" ", "_"), i);
            }

            initialized = true;
        }

        public dynamic NextRow()
        {
            if (!initialized)
            {
                throw new InvalidOperationException("Must call Initiaize() before looping through rows");
            }

            var row = reader.NextRow();
            if (row == null)
            {
                return null;
            }

            if (reader.ReuseRow)
            {
                if (dynamicRow == null)
                {
                    dynamicRow = new DynamicDelimitedRow(headers);
                }
                dynamicRow.SetData(row);
                return dynamicRow;
            }
            else
            {
                return new DynamicDelimitedRow(headers, row);
            }
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

        public IEnumerator<dynamic> GetEnumerator()
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

        public class DynamicDelimitedRow : DynamicObject
        {
            private readonly IDictionary<string, int> columns;
            private IDelimitedRow data;

            public DynamicDelimitedRow(IDictionary<string, int> columns)
            {
                this.columns = columns;
            }

            public DynamicDelimitedRow(IDictionary<string, int> columns, IDelimitedRow data)
                : this(columns)
            {
                if (data.Length != columns.Count)
                {
                    throw new InvalidOperationException("Length of data array: [" + data.Length + "] does not match number of expected columns: [" + columns.Count + "]");
                }
                this.data = data;
            }

            public void SetData(IDelimitedRow data)
            {
                if (data.Length != columns.Count)
                {
                    throw new InvalidOperationException("Length of data array: [" + data.Length + "] does not match number of expected columns: [" + columns.Count + "]");
                }
                this.data = data;
            }

            public override IEnumerable<string> GetDynamicMemberNames()
            {
                return this.columns.Keys;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if (this.columns.ContainsKey(binder.Name))
                {
                    result = this.data[this.columns[binder.Name]];
                    return true;
                }
                else
                {
                    throw new MissingMemberException("The column " + binder.Name + " is not mapped");
                }
            }
        }
    }
}
