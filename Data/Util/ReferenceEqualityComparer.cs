using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace InitiativeTracker.Data.Util
{
    public class ReferenceEqualityComparer<T> : IEqualityComparer<T>
    {
        static IEqualityComparer<T> _defaultComparer;
        public static IEqualityComparer<T> Default
        {
            get { return _defaultComparer ?? (_defaultComparer = new ReferenceEqualityComparer<T>()); }
        }

        public bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
