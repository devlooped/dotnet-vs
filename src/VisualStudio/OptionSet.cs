using System;
using System.Collections.Generic;
using System.Text;
using Mono.Options;

namespace VisualStudio
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
