using Orchard.Threading.Tasks;
using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
namespace Orchard.Threading
{
    public interface IAsyncDisposable {
        Task DisposeAsync();
    }

    public static class Async {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="resource"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static async Task<TReturn> Using<TResource, TReturn>(TResource resource, Func<TResource, Task<TReturn>> body)
            where TResource : IAsyncDisposable {
            Exception exception = null;
            TReturn result = default(TReturn);
            try {
                result = await body(resource).AnyContext();
            } catch (Exception ex) {
                exception = ex;
            }

            await resource.DisposeAsync().AnyContext();
            if (exception != null) {
                var info = ExceptionDispatchInfo.Capture(exception);
                info.Throw();
            }

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="resource"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static Task Using<TResource>(TResource resource, Func<Task> body) where TResource : IAsyncDisposable {
            return Using(resource, r => body());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="resource"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static Task Using<TResource>(TResource resource, Action body) where TResource : IAsyncDisposable {
            return Using(resource, r => {
                body();
                return Task.CompletedTask;
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="resource"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static Task Using<TResource>(TResource resource, Func<TResource, Task> body) where TResource : IAsyncDisposable {
            return Using(resource, async r => {
                await body(resource).AnyContext();
                return Task.CompletedTask;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="resource"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static Task<TReturn> Using<TResource, TReturn>(TResource resource, Func<Task<TReturn>> body) where TResource : IAsyncDisposable {
            return Using(resource, r => body());
        }
    }
}
