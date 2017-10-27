//Copyright (c) Orchard, Inc. All Rights Reserved.
//License: https://raw.github.com/Orchard/Orchard/master/license.txt

namespace Orchard.Domain.Entities
{
    //Allow Exceptions to Customize ResponseStatus returned
    public interface IResponseStatusConvertible
    {
        ResponseStatus ToResponseStatus();
    }
}