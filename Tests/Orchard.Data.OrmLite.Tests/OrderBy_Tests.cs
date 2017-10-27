using Autofac;
using NUnit.Framework;
using Orchard.Data.OrmLite.Tests.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.OrmLite.Tests
{
    [TestFixture]
    public class OrderBy_Tests : OrmLiteTestsBase
    {
        public override void Register(ContainerBuilder builder)
        {

        }
        private Domain.Repositories.IRepository<AdmArea> _admAreaRepository;
        private IQueryable<AdmArea> query;

        public override void Init()
        {
            base.Init();
            _admAreaRepository = _container.Resolve<Domain.Repositories.IRepository<AdmArea>>();
            Expression<Func<AdmArea, bool>> predicate = x => x.Id >= 100 && x.Id < 104;
            Action<Orderable<AdmArea>> orderAction = (o =>
            {
                o.Desc(x => x.AreaID, x => x.Id)
                 .Asc(x => x.CreateUser);
            });
            query = _admAreaRepository.GetList(predicate).AsQueryable();
        }
        [Test]
        public void ApplyOrderBy_Test()
        {
            var orderByExpressions = new IOrderByExpression<AdmArea>[] {
                new OrderByExpression<AdmArea,string>(u => u.AreaID),    // a string, asc
                new OrderByExpression<AdmArea, int>(u => u.Id, true)
            };
            //foreach (var item in orderByExpressions)
            //{
            //    Console.WriteLine(item.GetType());
            //    var tmpType = item.GetType();
            //    var properties = tmpType.GetProperties();
            //    foreach (var itemProperty in tmpType.GetProperties())
            //    {
            //        Console.WriteLine(itemProperty.Name+"item");
            //        Type genericType = typeof(OrderByExpression<,>);
            //    }
            //}
            ////创建表达式树
            //Expression<Func<int, bool>> expTree = num => num >= 5;
            ////获取输入参数
            //ParameterExpression param = expTree.Parameters[0];
            ////获取lambda表达式主题部分
            //BinaryExpression body = (BinaryExpression)expTree.Body;
            ////获取num>=5的右半部分
            //ConstantExpression right = (ConstantExpression)body.Right;
            ////获取num>=5的左半部分
            //ParameterExpression left = (ParameterExpression)body.Left;
            ////获取比较运算符
            //ExpressionType type = body.NodeType;
            //Console.WriteLine("解析后：{0}   {1}    {2}", left, type, right);

            Expression<Func<AdmArea, string>> expString = a => a.AreaID;
            Expression<Func<AdmArea, object>> expObject = a => a.AreaID;
            Console.WriteLine("expString:" + expString.Body + ",NodeType:" + expString.NodeType + ",Body.NodeType:" + expString.Body.NodeType + ",Type:" + expString.Type);
            Console.WriteLine("expObject:" + expObject.Body + ",NodeType:" + expObject.NodeType + ",Body.NodeType:" + expObject.Body.NodeType + ",Type:" + expObject.Type);
            SqlServerExpression<AdmArea> sqlServerExpression = new SqlServerExpression<AdmArea>(SqlServer2016OrmLiteDialectProvider.Instance);
            Console.WriteLine("SqlServerExpression,expString:" + sqlServerExpression.Visit(expString));
            Console.WriteLine("SqlServerExpression,expObject:" + sqlServerExpression.Visit(expObject));
            var parameter = Expression.Parameter(typeof(AdmArea));
            var property = typeof(AdmArea).GetProperty(expString.Body.ToString());
            //var lambdaBody = ConvertToType(parameter, property,TypeCode.Object);
            //var lambda = Expression.Lambda<Func<AdmArea, object>>(lambdaBody, parameter);
            //var valueAsDecimal = (decimal)lambda.Compile().Invoke(new AdmArea { ValueAsString = "42" });
            //Assert.AreEqual(42m, valueAsDecimal);
            // var convertedLambda = GetConvertedSource(parameter,property,TypeCode.Object);

            Console.WriteLine("expString:" + expString.Compile().ToString());
            Console.WriteLine("expObject:" + expObject.Compile().ToString());
            //Assert.AreEqual(convertedLambda, expObject);
            var newExp =expString.ToObjectTypeExpression();
            Console.WriteLine("newExp:" + newExp.Body + ",NodeType:" + newExp.NodeType + ",Body.NodeType:" + newExp.Body.NodeType + ",Type:" + newExp.Type);
            Console.WriteLine("newExp,PropertyName:" + newExp.GetPropertyInfo()+ ",expObject,PropertyName:" + expObject.GetPropertyInfo());


        }

        public MethodCallExpression ConvertToType(
    ParameterExpression sourceParameter,
    PropertyInfo sourceProperty,
    TypeCode typeCode)
        {
            var sourceExpressionProperty = Expression.Property(sourceParameter, sourceProperty);
            var changeTypeMethod = typeof(Convert).GetMethod("ChangeType", new Type[] { typeof(object), typeof(TypeCode) });
            var callExpressionReturningObject = Expression.Call(changeTypeMethod, sourceExpressionProperty, Expression.Constant(typeCode));
            return callExpressionReturningObject;
        }
        private Expression GetConvertedSource(ParameterExpression sourceParameter,
                                             PropertyInfo sourceProperty,
                                             TypeCode typeCode)
        {
            var sourceExpressionProperty = Expression.Property(sourceParameter,
                                                               sourceProperty);

            var changeTypeCall = Expression.Call(typeof(Convert).GetMethod("ChangeType",
                                                                   new[] { typeof(object),
                                                            typeof(TypeCode) }),
                                                                    sourceExpressionProperty,
                                                                    Expression.Constant(typeCode)
                                                                    );

            Expression convert = Expression.Convert(changeTypeCall,
                                                    Type.GetType("System." + typeCode));

            var convertExpr = Expression.Condition(Expression.Equal(sourceExpressionProperty,
                                                    Expression.Constant(null, sourceProperty.PropertyType)),
                                                    Expression.Default(Type.GetType("System." + typeCode)),
                                                    convert);



            return convertExpr;
        }

    }
    public static class ExpressionHelper
    {
        public static Expression<Func<object, object>> ConvertParameterToObject<T>(this Expression<Func<T, object>> source)
        {
            return source.ReplaceParameterWithBase<T, object, object>();
        }

        public static Expression<Func<TBase, TResult>> ReplaceParameterWithBase<T, TResult, TBase>(this Expression<Func<T, TResult>> lambda)
            where T : TBase
        {
            var param = lambda.Parameters.Single();
            return (Expression<Func<TBase, TResult>>)
                ParameterRebinder.ReplaceParameters(new Dictionary<ParameterExpression, ParameterExpression>
                                                    {
                                                    { param, Expression.Parameter(typeof (TBase), param.Name) }
                                                    }, lambda.Body);
        }

    }


    public class ParameterRebinder : ExpressionVisitor
    {

        private readonly Dictionary<ParameterExpression, ParameterExpression> map;
        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {

            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();

        }
        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }
        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if (map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);

        }

    }

}
