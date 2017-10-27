using System;
using System.Data.Entity.ModelConfiguration;

namespace Orchard.Data.EntityFramework
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    [Serializable]
    public class EntityTypeConfigurationBase<T> : EntityTypeConfiguration<T> where T : class
    {
        protected EntityTypeConfigurationBase()
        {
            PostInitialize();
        }

        /// <summary>
        /// Developers can override this method in custom partial classes
        /// in order to add some custom initialization code to constructors
        /// </summary>
        protected virtual void PostInitialize()
        {

        }
    }
}
