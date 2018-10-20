using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Utilities
{
    /// <summary>
    /// A simple class that enables to make dynamic indexed propterties.
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="K"></typeparam>
    public class ArrayProperty<K, V>
    {
        private Func<K, V> getPredicate;
        private Action<K, V> setPredicate;

        public V this[K key]
        {
            get
            {
                if (getPredicate == null)
                    throw new NotImplementedException();
                else
                    return getPredicate(key);
            }
            set
            {
                if (setPredicate == null)
                    throw new NotImplementedException();
                else
                    setPredicate(key, value);
            }
        }

        public ArrayProperty(Func<K, V> getMethod, Action<K, V> setMethod)
        {
            getPredicate = getMethod;
            setPredicate = setMethod;
        }

        public ArrayProperty(Func<K, V> getMethod) : this(getMethod, null) { }

        public Dictionary<K,V> AsDictionary()
        {
            if (typeof(K).IsEnum)
            {
                var enumValues = (K[])Enum.GetValues(typeof(K));
                var itemList = new Dictionary<K, V>();
                foreach (var val in enumValues)
                    itemList.Add(val, this[val]);

                return itemList;
            }

            throw new NotSupportedException();
        }

        public List<V> AsList()
        {
            if (typeof(K).IsEnum)
            {
                var enumValues = (K[])Enum.GetValues(typeof(K));
                var itemList = new List<V>();
                foreach (var val in enumValues)
                    itemList.Add(this[val]);

                return itemList;
            }

            throw new NotSupportedException();
        }
    }
}
