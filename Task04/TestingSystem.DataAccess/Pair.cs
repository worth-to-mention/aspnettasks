using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingSystem.DataAccess
{
    /// <summary>
    /// Represent a pair of related values.
    /// </summary>
    /// <typeparam name="T">Type of the first value.</typeparam>
    /// <typeparam name="V">Tupe of the second falue.</typeparam>
    public class Pair<T, V>
    {
        private T first;
        private V second;
        /// <summary>
        /// Constructs an instance of Pair<T, V> with default values
        /// for specified types.
        /// </summary>
        public Pair()
        {
            first = default(T);
            second = default(V);
        }
        /// <summary>
        /// Constructs an instance of Pair<T, V>
        /// with specifeid values.
        /// </summary>
        /// <param name="first">First value</param>
        /// <param name="second">Second value.</param>
        public Pair(T first, V second)
        {
            this.first = first;
            this.second = second;
        }
        /// <summary>
        /// Gets or sets a first value of a pair.
        /// </summary>
        public T First
        {
            get { return first; }
            set { first = value; }
        }
        /// <summary>
        /// Gets or sets a second value of a pair.
        /// </summary>
        public V Second
        {
            get { return second; }
            set { second = value; }
        }
    }
}
