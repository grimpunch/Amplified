using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Amplified.Utils
{
    public static class Helpers
    {
        public static void OnPropertyChanged<T>(this PropertyChangedEventHandler handler, Expression<Func<T>> property)
        {
            LambdaExpression lambda = Expression.Lambda(((MemberExpression)property.Body).Expression);
            Delegate vmFunc = lambda.Compile();
            object sender = vmFunc.DynamicInvoke();
            
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            if (propertyInfo != null)
            {
                handler?.Invoke(sender, new PropertyChangedEventArgs(propertyInfo.Name));
            }
        }
    }
}
