using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Orchard.Data.NHibernate.Extensions
{
	sealed class ActionQueryable<T> : IOrderedQueryable<T>, IEnumerator<T>
	{
		private IQueryable<T> queryable;

		private IEnumerator<T> enumerator;

		private Action<T> eachAction;

		private Action startAction;

		private Action endAction;

		public ActionQueryable(IQueryable<T> queryable, Action<T> eachAction, Action startAction = null, Action endAction = null)
		{
			this.startAction = startAction;
			this.endAction = endAction;
			this.eachAction = eachAction;
			this.queryable = queryable;
			this.enumerator = queryable.GetEnumerator();

			if (this.startAction != null)
			{
				this.startAction();
			}
		}

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return (this);
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (this);
		}

		#endregion

		#region IQueryable Members

		public Type ElementType
		{
			get { return (this.queryable.ElementType); }
		}

		public System.Linq.Expressions.Expression Expression
		{
			get { return (this.queryable.Expression); }
		}

		public IQueryProvider Provider
		{
			get { return (this.queryable.Provider); }
		}

		#endregion

		#region IEnumerator<T> Members

		public T Current
		{
			get
			{
				T current = this.enumerator.Current;

				if (this.eachAction != null)					
				{
					if (Object.ReferenceEquals(current, default(T)) == false)
					{
						this.eachAction(current);
					}
				}

				return (current);
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			this.enumerator.Dispose();
			this.enumerator = null;
			this.queryable = null;
			this.eachAction = null;
			this.startAction = null;
			this.endAction = null;
		}

		#endregion

		#region IEnumerator Members

		Object IEnumerator.Current
		{
			get { return (this.enumerator.Current); }
		}

		public Boolean MoveNext()
		{
			var moveNext = this.enumerator.MoveNext();

			if (moveNext == false)
			{
				if (this.endAction != null)
				{
					this.endAction();
				}
			}

			return (moveNext);
		}

		public void Reset()
		{
			this.enumerator.Reset();
		}

		#endregion
	}

}
