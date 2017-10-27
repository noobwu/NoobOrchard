using System;
namespace Orchard.Caching.Memcached
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class MemcachedValueWrapper
    {
        public Type ValueType { get; set; }

        [NonSerialized] private object _value;

        /// <summary>
        /// 
        /// </summary>
        public object Value
        {
            get
            {
                return _value;
            }
        }

        public MemcachedValueWrapper() {}

        public MemcachedValueWrapper(object value)
        {
            if (value == null) return;
            ValueType = value.GetType();
            _value = value;
        }
    }
}