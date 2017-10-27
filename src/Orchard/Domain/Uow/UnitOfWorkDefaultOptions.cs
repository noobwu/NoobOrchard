using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;

namespace Orchard.Domain.Uow
{
    public class UnitOfWorkDefaultOptions 
    {
        public TransactionScopeOption Scope { get; set; }

        /// <inheritdoc/>
        public bool IsTransactional { get; set; }

        /// <inheritdoc/>
        public TimeSpan? Timeout { get; set; }

#if NET46
        /// <inheritdoc/>
        public bool IsTransactionScopeAvailable { get; set; }
#endif

        /// <inheritdoc/>
        public IsolationLevel? IsolationLevel { get; set; }


        public UnitOfWorkDefaultOptions()
        {
            IsTransactional = true;
            Scope = TransactionScopeOption.Required;

#if NET46
            IsTransactionScopeAvailable = true;
#endif
        }
    }
}