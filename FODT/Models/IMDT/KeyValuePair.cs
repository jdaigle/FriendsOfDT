using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Models.IMDT
{
    [Serializable]
    public class KeyValuePair<K, V>
    {
        public K Key { get; set; }
        public V Value { get; set; }
    }
}