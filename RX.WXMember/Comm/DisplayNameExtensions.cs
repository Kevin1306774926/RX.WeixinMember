using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace RX.WXMember.Comm
{
    /// <summary>
    /// 大叔为DisplayName进行了扩展
    /// </summary>
    public static class DisplayNameExtensions
    {

        /// <summary>
        /// 显示字段的名称DisplayName的值
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="html"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString DisplayNameFor<TModel, TValue>(this HtmlHelper<PagedList<TModel>> html, Expression<Func<TModel, TValue>> expression)
        {
            Type t = typeof(TModel);
            // string propertyName = GetPropertyName<TModel, TValue>(expression);

            var complex = ExpressionHelper.GetExpressionText(expression).Split('.');
            string propertyName = complex.Last();
            if (complex.Count() > 1)
            {
                t = t.GetProperty(complex[complex.Length - 2]).PropertyType;
            }
            var p = t.GetProperty(propertyName);
            if (p != null)
            {
                var attr1 = p.GetCustomAttribute(typeof(DisplayNameAttribute));
                var attr2 = p.GetCustomAttribute(typeof(DisplayAttribute));
                if (attr1 != null)
                {
                    return MvcHtmlString.Create(((System.ComponentModel.DisplayNameAttribute)attr1).DisplayName);
                }
                if (attr2 != null)
                {
                    return MvcHtmlString.Create(((DisplayAttribute)attr2).Name);
                }
            }
            return MvcHtmlString.Create(string.Empty);
        }

        public static MvcHtmlString DisplayNameFor<TModel, TEnumerable, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, PagedList<TEnumerable>>> enumerableExpression, Expression<Func<TEnumerable, TValue>> valueExpression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(valueExpression, new ViewDataDictionary<TEnumerable>());
            string displayName = metadata.DisplayName ?? metadata.PropertyName ?? ExpressionHelper.GetExpressionText(valueExpression).Split('.').Last();
            return new MvcHtmlString(HttpUtility.HtmlEncode(displayName));
        }


    }
}