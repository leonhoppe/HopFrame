using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace HopFrame.Tests.Web.Helpers;

public static class EnumerableExtensions {

    public static AsyncQueryable AsAsyncQueryable(this IEnumerable<object> enumerable) {
        return new AsyncQueryable(enumerable);
    }

    public class AsyncQueryable(IEnumerable<object> enumerable) : IQueryable<object>, IAsyncEnumerable<object> {
        public IEnumerator<object> GetEnumerator() {
            return enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public Type ElementType { get; } = typeof(object);
        
        public Expression Expression { get; }
        
        public IQueryProvider Provider { get; }
        
        public IAsyncEnumerator<object> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken()) {
            return Enumerate().GetAsyncEnumerator(cancellationToken);
        }

        private async IAsyncEnumerable<object> Enumerate() {
            foreach (var o in enumerable) {
                yield return o;
            }
        }
    }
    
}