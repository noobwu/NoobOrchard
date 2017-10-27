//Copyright (c) Orchard, Inc. All Rights Reserved.
//License: https://raw.github.com/Orchard/Orchard/master/license.txt

using System.Collections.Generic;

namespace Orchard.Domain.Entities
{
    public interface IHasNamedCollection<T> : IHasNamed<ICollection<T>>
    {
    }
}