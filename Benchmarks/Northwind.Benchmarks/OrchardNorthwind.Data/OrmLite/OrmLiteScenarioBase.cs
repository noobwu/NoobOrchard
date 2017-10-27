using System;
using System.Data;
using ServiceStack.OrmLite;
using OrchardNorthwind.Data.Scenarios;

namespace OrchardNorthwind.Data.OrmLite
{
	public abstract class OrmLiteScenarioBase
        : ScenarioBase, IDisposable
	{
        public OrmLiteConnectionFactory ConnectionFactory { get; set; }
        //public string ConnectionString { get; set; }

		protected int Iteration;
		public bool IsFirstRun
		{
			get
			{
				return this.Iteration == 0;
			}
		}

        private IDbConnection db;
        protected IDbConnection Db
		{
			get
			{
                if (db == null)
                {
                    //var connStr = ConnectionString;
                    //db = connStr.OpenDbConnection();
                    db = ConnectionFactory.OpenDbConnection();
                }
				return db;
			}
		}

		public override void Run()
		{
            if (Db.State == ConnectionState.Broken || Db.State == ConnectionState.Closed)
            {
                Db.Open();
            }
            Run(Db);
			this.Iteration++;
		}

        protected abstract void Run(IDbConnection db);

		public void Dispose()
		{
			if (this.db == null) return;

			try
			{
				this.db.Dispose();
				this.db = null;
			}
			finally
			{
			}
		}
	}
}