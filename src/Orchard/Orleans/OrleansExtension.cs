using Newtonsoft.Json.Linq;
using Orchard.Data;
using Serialize.Linq.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Orleans
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class ExpressionExtension
    {
        /// <summary>
        /// Predicate JObject
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static JObject ToJObject<TEntity>(this Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            string predicateJson = JsonSerializerUtils.SerializeText(predicate);
            if (string.IsNullOrEmpty(predicateJson))
            {
                return null;
            }
            return JObject.Parse(predicateJson);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static JObject ToJObject<TEntity,TProperty>(this Expression<Func<TEntity,TProperty>> expression) where TEntity : class
        {
            string predicateJson = JsonSerializerUtils.SerializeText(expression);
            if (string.IsNullOrEmpty(predicateJson))
            {
                return null;
            }
            return JObject.Parse(predicateJson);
        }
        /// <summary>
        /// Order By JObject Array
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="orderByExpressions"></param>
        /// <returns></returns>
        public static JObject[] ToJObjectArray<TEntity>(this IOrderByExpression<TEntity>[] orderByExpressions) where TEntity : class
        {
            if (orderByExpressions == null || orderByExpressions.Length == 0) return null;
            List<JObject> orderByList = new List<JObject>();
            foreach (var item in orderByExpressions)
            {
                orderByList.Add(item.ToJObject());
            }
            return orderByList.ToArray();
        }
        /// <summary>
        /// Update JObject
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="updateExpression">updateExpression</param>
        /// <returns></returns>
        public static JObject ToJObject<TEntity>(this Expression<Func<TEntity, TEntity>> updateExpression) where TEntity : class
        {
            string predicateJson = JsonSerializerUtils.SerializeText(updateExpression);
            if (string.IsNullOrEmpty(predicateJson))
            {
                return null;
            }
            return JObject.Parse(predicateJson);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static partial class OrleansJsonExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        public static Expression<Func<TEntity, bool>> ToPredicate<TEntity>(this JObject jsonObject) where TEntity : class
        {
            if (jsonObject == null) return null;
           var result = JsonSerializerUtils.DeserializeJObject(jsonObject);
            if (result == null)
            {
                return null;
            }
            else
            {
                return result as Expression<Func<TEntity, bool>>;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        public static IOrderByExpression<TEntity> ToOrderBy<TEntity>(this JObject jsonObject) where TEntity : class
        {
            if (jsonObject == null) return null;
            var expressionTxt = jsonObject["expression"].ToString();
            if (string.IsNullOrEmpty(expressionTxt)) return null;
            var descending = (bool)jsonObject["descending"];
            var strPropertyType = jsonObject["propertyType"].ToString();
            var expression = JsonSerializerUtils.DeserializeText(expressionTxt);
            if (expression == null)
            {
                return null;
            }
            else
            {
                Type genericType = typeof(OrderByExpression<,>);
                Type propertyType = Type.GetType(strPropertyType);
                Type[] typeArgs = { typeof(TEntity), propertyType };
                Type implementType = genericType.MakeGenericType(typeArgs);
                //var tmpOrderByExpression = expression as Expression<Func<TEntity,int>>;
                //return new OrderByExpression<TEntity, int>(tmpOrderByExpression, descending);
                //var instance= Activator.CreateInstance(implementType, new object[] { expression, descending });
                #region  动态的泛型类型 暂时想不到更好的办法
                if (propertyType == typeof(int))
                {
                    return GetOrderByExpression<TEntity, int>(expression, descending);
                }
                if (propertyType == typeof(uint))
                {
                    return GetOrderByExpression<TEntity, uint>(expression, descending);
                }
                else if (propertyType == typeof(short))
                {
                    return GetOrderByExpression<TEntity, short>(expression, descending);
                }
                else if (propertyType == typeof(ushort))
                {
                    return GetOrderByExpression<TEntity, ushort>(expression, descending);
                }
                else if (propertyType == typeof(long))
                {
                    return GetOrderByExpression<TEntity, long>(expression, descending);
                }
                else if (propertyType == typeof(ulong))
                {
                    return GetOrderByExpression<TEntity, ulong>(expression, descending);
                }
                else if (propertyType == typeof(string))
                {
                    return GetOrderByExpression<TEntity, string>(expression, descending);
                }
                else if (propertyType == typeof(double))
                {
                    return GetOrderByExpression<TEntity, double>(expression, descending);
                }
                else if (propertyType == typeof(decimal))
                {
                    return GetOrderByExpression<TEntity, decimal>(expression, descending);
                }
                else if (propertyType == typeof(float))
                {
                    return GetOrderByExpression<TEntity, float>(expression, descending);
                }
                else if (propertyType == typeof(byte))
                {
                    return GetOrderByExpression<TEntity, byte>(expression, descending);
                }
                else if (propertyType == typeof(sbyte))
                {
                    return GetOrderByExpression<TEntity, sbyte>(expression, descending);
                }
                else if (propertyType == typeof(DateTime))
                {
                    return GetOrderByExpression<TEntity, DateTime>(expression, descending);
                }
                else if (propertyType == typeof(Guid))
                {
                    return GetOrderByExpression<TEntity, Guid>(expression, descending);
                }
                else
                {
                    return null;
                }
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expression"></param>
        /// <param name="descending"></param>
        /// <returns></returns>
        private static IOrderByExpression<TEntity> GetOrderByExpression<TEntity, TProperty>(Expression expression, bool descending) where TEntity : class
        {
            var tmpOrderByExpression = expression as Expression<Func<TEntity, TProperty>>;
            return new OrderByExpression<TEntity, TProperty>(tmpOrderByExpression, descending);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static IOrderByExpression<TEntity>[] ToOrderByArray<TEntity>(this JObject[] jsonObjects) where TEntity : class
        {
            if (jsonObjects == null || jsonObjects.Length == 0)
            {
                return null;
            }
            List<IOrderByExpression<TEntity>> orderByList = new List<IOrderByExpression<TEntity>>();
            foreach (var item in jsonObjects)
            {
                orderByList.Add(item.ToOrderBy<TEntity>());
            }
            return orderByList.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        public static Expression<Func<TEntity, TEntity>> ToUpdateExpression<TEntity>(this JObject jsonObject) where TEntity : class
        {
            if (jsonObject == null) return null;
            var result = JsonSerializerUtils.DeserializeJObject(jsonObject);
            if (result == null)
            {
                return null;
            }
            else
            {
                return result as Expression<Func<TEntity, TEntity>>;
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public partial class JsonSerializerUtils
    {
        private static ExpressionSerializer serializer = new ExpressionSerializer(new Serialize.Linq.Serializers.JsonSerializer());

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static string SerializeText<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return serializer.SerializeText(predicate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static string SerializeText<TEntity,TProperty>(Expression<Func<TEntity, TProperty>> expression) where TEntity : class
        {
            return serializer.SerializeText(expression);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        public static Expression DeserializeJObject(JObject jsonObject)
        {
            if (jsonObject==null)
            {
                return null;
            }
            string strJson = jsonObject.ToString();
            if (string.IsNullOrEmpty(strJson)) return null;
            return serializer.DeserializeText(strJson);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        public static Expression DeserializeText(string strJson)
        {

            if (string.IsNullOrEmpty(strJson)) return null;
            return serializer.DeserializeText(strJson);
        }
    }
}
