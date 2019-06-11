using System;
using System.Collections.Generic;

namespace WebStore
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> entities, Action<T> action)
        {
            foreach (var entity in entities) action(entity);
        }
    }
}