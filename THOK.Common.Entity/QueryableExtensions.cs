using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Data.Objects;

namespace Entities.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<TElement> WhereIn<TElement, TValue>(this IQueryable<TElement> source, Expression<Func<TElement, TValue>> propertySelector, params TValue[] values) where TElement : class
        {
            return source.Where(BuildWhereInExpression(propertySelector, values));
        }

        public static IQueryable<TElement> WhereNotIn<TElement, TValue>(this IQueryable<TElement> source,Expression<Func<TElement, TValue>> propertySelector, params TValue[] values) where TElement : class
        {
            return source.Where(BuildWhereNotInExpression(propertySelector, values));
        }

        public static Expression<Func<TElement, bool>> WhereOrIn<TElement, TValue>(this IQueryable<TElement> source, Expression<Func<TElement, TValue>> propertySelector, IEnumerable<TValue> values, Expression<Func<TElement, bool>> left) where TElement : class
        {
            return BuildWhereOrInExpression(propertySelector, values, left);
        }

        public static Expression<Func<TElement, bool>> WhereOrNotIn<TElement, TValue>(this IQueryable<TElement> source, Expression<Func<TElement, TValue>> propertySelector, IEnumerable<TValue> values, Expression<Func<TElement, bool>> left) where TElement : class
        {
            return BuildWhereOrNotInExpression(propertySelector, values, left);
        }

        public static Expression<Func<TElement, bool>> WhereAndIn<TElement, TValue>(this IQueryable<TElement> source, Expression<Func<TElement, TValue>> propertySelector, IEnumerable<TValue> values, Expression<Func<TElement, bool>> left) where TElement : class
        {
            return BuildWhereAndInExpression(propertySelector, values, left);
        }

        public static Expression<Func<TElement, bool>> WhereAndNotIn<TElement, TValue>(this IQueryable<TElement> source, Expression<Func<TElement, TValue>> propertySelector, IEnumerable<TValue> values, Expression<Func<TElement, bool>> left) where TElement : class
        {
            return BuildWhereAndNotInExpression(propertySelector, values, left);
        }

        public static Expression<Func<TElement, bool>> BuildWhereInExpression<TElement, TValue>(Expression<Func<TElement, TValue>> propertySelector, IEnumerable<TValue> values)
        {
            ParameterExpression p = propertySelector.Parameters.Single();
            if (!values.Any())
                return e => false;

            var equals = values.Select(value => (Expression)Expression.Equal(propertySelector.Body, Expression.Constant(value, typeof(TValue))));
            var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));

            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }

        public static Expression<Func<TElement, bool>> BuildWhereNotInExpression<TElement, TValue>(Expression<Func<TElement, TValue>> propertySelector, IEnumerable<TValue> values)
        {
            ParameterExpression p = propertySelector.Parameters.Single();
            if (!values.Any())
                return e => true;

            var equals = values.Select(value => (Expression)Expression.Equal(propertySelector.Body, Expression.Constant(value, typeof(TValue))));
            var body = Expression.Not(equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal)));

            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }

        public static Expression<Func<TElement, bool>> BuildWhereOrInExpression<TElement, TValue>(Expression<Func<TElement, TValue>> propertySelector, IEnumerable<TValue> values, Expression<Func<TElement, bool>> left)
        {
            ParameterExpression p = propertySelector.Parameters.Single();
            if (!values.Any())
                return e => false;

            var equals = values.Select(value => (Expression)Expression.Equal(propertySelector.Body, Expression.Constant(value, typeof(TValue))));
            var body = Expression.Or(left as BinaryExpression, equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal)));

            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }

        public static Expression<Func<TElement, bool>> BuildWhereOrNotInExpression<TElement, TValue>(Expression<Func<TElement, TValue>> propertySelector, IEnumerable<TValue> values, Expression<Func<TElement, bool>> left)
        {
            ParameterExpression p = propertySelector.Parameters.Single();
            if (!values.Any())
                return e => true;

            var equals = values.Select(value => (Expression)Expression.Equal(propertySelector.Body, Expression.Constant(value, typeof(TValue))));
            var body = Expression.Or(left as BinaryExpression, Expression.Not(equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal))));

            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }

        public static Expression<Func<TElement, bool>> BuildWhereAndInExpression<TElement, TValue>(Expression<Func<TElement, TValue>> propertySelector, IEnumerable<TValue> values, Expression<Func<TElement, bool>> left)
        {
            ParameterExpression p = propertySelector.Parameters.Single();
            if (!values.Any())
                return e => false;

            var equals = values.Select(value => (Expression)Expression.Equal(propertySelector.Body, Expression.Constant(value, typeof(TValue))));
            var body = Expression.And(left as BinaryExpression, equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal)));

            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }

        public static Expression<Func<TElement, bool>> BuildWhereAndNotInExpression<TElement, TValue>(Expression<Func<TElement, TValue>> propertySelector, IEnumerable<TValue> values, Expression<Func<TElement, bool>> left)
        {
            ParameterExpression p = propertySelector.Parameters.Single();
            if (!values.Any())
                return e => true;

            var equals = values.Select(value => (Expression)Expression.Equal(propertySelector.Body, Expression.Constant(value, typeof(TValue))));
            var body = Expression.And(left as BinaryExpression, Expression.Not(equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal))));

            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }
    }
}
