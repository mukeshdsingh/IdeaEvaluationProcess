﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdeaEvaluationProcess.Helper
{
  public static class LinqExtension
  {
    public static IEnumerable<TResult> LeftExcludingJoin<TSource, TInner, TKey, TResult>(this IEnumerable<TSource> source,
                                                          IEnumerable<TInner> inner,
                                                          Func<TSource, TKey> pk,
                                                          Func<TInner, TKey> fk,
                                                          Func<TSource, TInner, TResult> result)
                                        where TSource : class where TInner : class
    {
      IEnumerable<TResult> _result = Enumerable.Empty<TResult>();

      _result = from s in source
                join i in inner
                on pk(s) equals fk(i) into joinData
                from left in joinData.DefaultIfEmpty()
                where left == null
                select result(s, left);

      return _result;
    }

    public static IEnumerable<TResult> LeftJoin<TSource, TInner, TKey, TResult>(this IEnumerable<TSource> source,
                                                 IEnumerable<TInner> inner,
                                                 Func<TSource, TKey> pk,
                                                 Func<TInner, TKey> fk,
                                                 Func<TSource, TInner, TResult> result)
                                        where TSource : class where TInner : class
    {
      IEnumerable<TResult> _result = Enumerable.Empty<TResult>();

      _result = from s in source
                join i in inner
                on pk(s) equals fk(i) into joinData
                from left in joinData.DefaultIfEmpty()
                select result(s, left);

      return _result;
    }

    public static IEnumerable<TResult> RightJoin<TSource, TInner, TKey, TResult>(this IEnumerable<TSource> source,
                                                  IEnumerable<TInner> inner,
                                                  Func<TSource, TKey> pk,
                                                  Func<TInner, TKey> fk,
                                                  Func<TSource, TInner, TResult> result)
                                        where TSource : class where TInner : class
    {
      IEnumerable<TResult> _result = Enumerable.Empty<TResult>();

      _result = from i in inner
                join s in source
                on fk(i) equals pk(s) into joinData
                from right in joinData.DefaultIfEmpty()
                select result(right, i);

      return _result;
    }

    public static IEnumerable<TResult> FullOuterJoinJoin<TSource, TInner, TKey, TResult>(this IEnumerable<TSource> source,
                                                          IEnumerable<TInner> inner,
                                                          Func<TSource, TKey> pk,
                                                          Func<TInner, TKey> fk,
                                                          Func<TSource, TInner, TResult> result)
                                        where TSource : class where TInner : class
    {

      var left = source.LeftJoin(inner, pk, fk, result).ToList();
      var right = source.RightJoin(inner, pk, fk, result).ToList();

      return left.Union(right);
    }
  }
}
