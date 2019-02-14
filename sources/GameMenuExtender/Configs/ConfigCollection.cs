using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Configs
{
    internal class ConfigCollection<T> : IList<T> where T : ConfigBase
    {
        public ConfigManager Owner { get; }

        public int Count => ((IList<T>)Items).Count;

        public bool IsReadOnly => ((IList<T>)Items).IsReadOnly;

        public T this[int index] { get => ((IList<T>)Items)[index]; set => ((IList<T>)Items)[index] = value; }

        private List<T> Items;

        public ConfigCollection(ConfigManager owner)
        {
            Owner = owner;
            Items = new List<T>();
        }

        public int IndexOf(T item)
        {
            return ((IList<T>)Items).IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ((IList<T>)Items).Insert(index, item);
            item.Manager = Owner;
        }

        public void RemoveAt(int index)
        {
            ((IList<T>)Items).RemoveAt(index);
        }

        public void Add(T item)
        {
            ((IList<T>)Items).Add(item);
            item.Manager = Owner;
        }

        public void Clear()
        {
            ((IList<T>)Items).Clear();
        }

        public bool Contains(T item)
        {
            return ((IList<T>)Items).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((IList<T>)Items).CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return ((IList<T>)Items).Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IList<T>)Items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<T>)Items).GetEnumerator();
        }
    }
}
