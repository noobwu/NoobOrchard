using NUnit.Framework;
using ServiceStack;
using ServiceStack.Common.Tests.Models;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using ServiceStack.Text;
using System;
using System.Data;
using Autofac;

namespace Orchard.Data.OrmLite.Tests.Uow
{
    [TestFixture]
    public class OrmLiteTransactionTests
        : OrmLiteNUnitTestBase
    {
        [Test]
        public void Transaction_commit_persists_data_to_the_db()
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                db.DropAndCreateTable<ModelWithIdAndName>();
                db.Insert(new ModelWithIdAndName(1));

                Assert.That(((OrmLiteConnection)db).Transaction, Is.Null);
                //using (var trans = new TransactionScope())
                //{
                //    long result0 = db.Insert(new ModelWithIdAndName(2));
                //    long result1 = db.Insert(ModelWithIdAndName.Create(3));
                //    long result2 = db.Insert(ModelWithIdAndName.Create(3));
                //    if (result0 > 0 && result1 > 0 && result2 > 0)
                //    {
                //        trans.Complete();
                //    }
                //}
                //using (var dbTrans = db.OpenTransaction())
                //{
                //    long result0 = db.Insert(new ModelWithIdAndName(2));
                //    long result1 = db.Insert(ModelWithIdAndName.Create(3));
                //    long result2 = db.Insert(ModelWithIdAndName.Create(3));
                //    if (result0 > 0 && result1 > 0 && result2 > 0)
                //    {
                //        dbTrans.Commit();
                //    }
                //}
                //using (var dbTrans = db.OpenTransaction())
                //{
                //    Assert.That(((OrmLiteConnection)db).Transaction, Is.Not.Null);

                //    db.Insert(new ModelWithIdAndName(2));
                //    db.Insert(new ModelWithIdAndName(3));

                //    var rowsInTrans = db.Select<ModelWithIdAndName>();
                //    Assert.That(rowsInTrans, Has.Count.EqualTo(3));

                //    dbTrans.Commit();
                //}

                //UsingTransaction(connection =>
                //{
                //    connection.Insert(new ModelWithIdAndName(2));
                //    connection.Insert(new ModelWithIdAndName(3));
                //    connection.Insert(new ModelWithIdAndName(2));
                //});
                using (var dbTrans = db.OpenTransaction())
                {
                    try
                    {
                        long insertResult0 = AddTransactionAction(
                      connection => { return connection.Insert(new ModelWithIdAndName(2)); },
                      db);
                        long insertResult1 = AddTransactionAction(
                     connection => { return connection.Insert(new ModelWithIdAndName(2)); },
                     db);
                        if (insertResult0 > 0 && insertResult1 > 0)
                        {
                            dbTrans.Commit();
                        }
                        else
                        {
                            dbTrans.Rollback();
                        }
                    }
                    catch (Exception ex)
                    {
                        dbTrans.Rollback();
                        throw ex;
                    }


                }
                //var rows = db.Select<ModelWithIdAndName>();
                //Assert.That(rows, Has.Count.EqualTo(3));
            }
        }
        private void UsingTransaction(Action<IDbConnection> actions)
        {
            using (var dbConn = OpenDbConnection())
            {
                using (var trans = dbConn.OpenTransaction())
                {
                    try
                    {
                        actions(dbConn);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }
        private T AddTransactionAction<T>(Func<IDbConnection, T> func, IDbConnection dbConn)
        {
            return func(dbConn);
        }
        [Test]
        public void Transaction_rollsback_if_not_committed()
        {
            using (var db = OpenDbConnection())
            {
                db.DropAndCreateTable<ModelWithIdAndName>();
                db.Insert(new ModelWithIdAndName(1));

                using (var dbTrans = db.OpenTransaction())
                {
                    db.Insert(new ModelWithIdAndName(2));
                    db.Insert(new ModelWithIdAndName(3));

                    var rowsInTrans = db.Select<ModelWithIdAndName>();
                    Assert.That(rowsInTrans, Has.Count.EqualTo(3));
                }

                var rows = db.Select<ModelWithIdAndName>();
                Assert.That(rows, Has.Count.EqualTo(1));
            }
        }

        [Test]
        public void Transaction_rollsback_transactions_to_different_tables()
        {
            using (var db = OpenDbConnection())
            {
                db.DropAndCreateTable<ModelWithIdAndName>();
                db.DropAndCreateTable<ModelWithFieldsOfDifferentTypes>();
                db.DropAndCreateTable<ModelWithOnlyStringFields>();

                db.Insert(new ModelWithIdAndName(1));

                using (var dbTrans = db.OpenTransaction())
                {
                    db.Insert(new ModelWithIdAndName(2));
                    db.Insert(ModelWithFieldsOfDifferentTypes.Create(3));
                    db.Insert(ModelWithOnlyStringFields.Create("id3"));

                    Assert.That(db.Select<ModelWithIdAndName>(), Has.Count.EqualTo(2));
                    Assert.That(db.Select<ModelWithFieldsOfDifferentTypes>(), Has.Count.EqualTo(1));
                    Assert.That(db.Select<ModelWithOnlyStringFields>(), Has.Count.EqualTo(1));
                }

                Assert.That(db.Select<ModelWithIdAndName>(), Has.Count.EqualTo(1));
                Assert.That(db.Select<ModelWithFieldsOfDifferentTypes>(), Has.Count.EqualTo(0));
                Assert.That(db.Select<ModelWithOnlyStringFields>(), Has.Count.EqualTo(0));
            }
        }

        [Test]
        public void Transaction_commits_inserts_to_different_tables()
        {
            using (var db = OpenDbConnection())
            {
                db.DropAndCreateTable<ModelWithIdAndName>();
                db.DropAndCreateTable<ModelWithFieldsOfDifferentTypes>();
                db.DropAndCreateTable<ModelWithOnlyStringFields>();

                db.Insert(new ModelWithIdAndName(1));

                using (var dbTrans = db.OpenTransaction())
                {
                    db.Insert(new ModelWithIdAndName(2));
                    db.Insert(ModelWithFieldsOfDifferentTypes.Create(3));
                    db.Insert(ModelWithOnlyStringFields.Create("id3"));

                    Assert.That(db.Select<ModelWithIdAndName>(), Has.Count.EqualTo(2));
                    Assert.That(db.Select<ModelWithFieldsOfDifferentTypes>(), Has.Count.EqualTo(1));
                    Assert.That(db.Select<ModelWithOnlyStringFields>(), Has.Count.EqualTo(1));

                    dbTrans.Commit();
                }

                Assert.That(db.Select<ModelWithIdAndName>(), Has.Count.EqualTo(2));
                Assert.That(db.Select<ModelWithFieldsOfDifferentTypes>(), Has.Count.EqualTo(1));
                Assert.That(db.Select<ModelWithOnlyStringFields>(), Has.Count.EqualTo(1));
            }
        }

        public class MyTable
        {
            [AutoIncrement]
            public int Id { get; set; }
            public string SomeTextField { get; set; }
        }


        [Test]
        public void Does_allow_setting_transactions_on_raw_DbCommands()
        {
            using (var db = OpenDbConnection())
            {
                db.DropAndCreateTable<MyTable>();

                using (var trans = db.OpenTransaction())
                {
                    db.Insert(new MyTable { SomeTextField = "Example" });

                    using (var dbCmd = db.CreateCommand())
                    {
                        dbCmd.Transaction = trans.ToDbTransaction();

                            dbCmd.CommandText = "INSERT INTO {0} ({1}) VALUES ('From OrmLite DB Command')"
                                .Fmt("MyTable".SqlTable(), "SomeTextField".SqlColumn());


                        dbCmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }

                Assert.That(db.Count<MyTable>(), Is.EqualTo(2));
            }
        }

        [Test]
        public void Can_use_OpenCommand_in_Transaction()
        {
            using (var db = OpenDbConnection())
            {
                db.DropAndCreateTable<MyTable>();

                using (var trans = db.OpenTransaction())
                {
                    db.Insert(new MyTable { SomeTextField = "Example" });

                    using (var dbCmd = db.OpenCommand())
                    {
                            dbCmd.CommandText = "INSERT INTO {0} ({1}) VALUES ('From OrmLite DB Command')"
                                .Fmt("MyTable".SqlTable(), "SomeTextField".SqlColumn());


                        dbCmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }

                Assert.That(db.Count<MyTable>(), Is.EqualTo(2));
            }
        }
        private IDbConnection OpenDbConnection()
        {
            return DbFactory.OpenDbConnection();
        }
        protected override void Register(ContainerBuilder builder)
        {
           
        }
    }
}
