using Mono.Options;

namespace Devlooped
{
    class OptionSet<T> : OptionSet
    {
        public OptionSet(T value = default)
        {
            Value = value;
        }

        public T Value { get; protected set; }
    }
}
