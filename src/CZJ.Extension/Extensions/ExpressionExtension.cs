namespace CZJ.Extension
{
    /// <summary>
    /// 表达式扩展
    /// </summary>
    public static class ExpressionExtension
    {
        public static TTarget Map<TSource, TTarget>(this TSource source)
           where TTarget : new()
        {
            var param = Expression.Parameter(typeof(TSource), "src");
            var bindings = new List<MemberBinding>();
            foreach (var targetProp in typeof(TTarget).GetProperties())
            {
                var sourceProp = typeof(TSource).GetProperty(targetProp.Name);
                if (sourceProp == null) continue;

                var sourceExpr = Expression.Property(param, sourceProp);
                // 处理自定义转换
                var converterAttr = targetProp.GetCustomAttribute<MapConvertAttribute>();
                var finalExpr = converterAttr != null
                    ? Expression.Call(Expression.Constant(converterAttr),
                        nameof(MapConvertAttribute.ConverterType), null, sourceExpr)
                    : (Expression)sourceExpr;
                bindings.Add(Expression.Bind(targetProp, finalExpr));
            }
            var lambda = Expression.Lambda<Func<TSource, TTarget>>(
                Expression.MemberInit(Expression.New(typeof(TTarget)), bindings), param);
            return lambda.Compile()(source);
        }

        /// <summary>
        /// 添加And条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(
        this Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second)
        {
            return first.AndAlso(second, Expression.AndAlso);
        }

        /// <summary>
        /// 添加Or条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.AndAlso(second, Expression.OrElse);
        }

        /// <summary>
        /// 合并表达式以及参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1"></param>
        /// <param name="expr2"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private static Expression<Func<T, bool>> AndAlso<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2,
        Func<Expression, Expression, BinaryExpression> func)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(
                func(left, right), parameter);
        }

        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression? node)
            {
                if (node == _oldValue)
                    return _newValue;
                return base.Visit(node);
            }
        }
    }
}
