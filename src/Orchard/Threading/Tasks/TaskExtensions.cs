using Nito.AsyncEx;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Orchard.Threading.Tasks
{
    public static class TaskExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="fn"></param>
        /// <param name="onUiThread"></param>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        public static Task<T> Success<T>(this Task<T> task, Action<T> fn,
            bool onUiThread = true,
            TaskContinuationOptions taskOptions = TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.OnlyOnRanToCompletion)
        {
            if (onUiThread)
            {
                var source = new CancellationToken();
                task.ContinueWith(t => fn(t.Result), source, taskOptions, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                task.ContinueWith(t => fn(t.Result), taskOptions);
            }
            return task;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="fn"></param>
        /// <param name="onUiThread"></param>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        public static Task Success(this Task task, Action fn,
            bool onUiThread = true,
            TaskContinuationOptions taskOptions = TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.OnlyOnRanToCompletion)
        {
            if (onUiThread)
            {
                var source = new CancellationToken();
                task.ContinueWith(t => fn(), source, taskOptions, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                task.ContinueWith(t => fn(), taskOptions);
            }
            return task;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="fn"></param>
        /// <param name="onUiThread"></param>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        public static Task<T> Error<T>(this Task<T> task, Action<Exception> fn,
            bool onUiThread = true,
            TaskContinuationOptions taskOptions = TaskContinuationOptions.OnlyOnFaulted)
        {
            if (onUiThread)
            {
                var source = new CancellationToken();
                task.ContinueWith(t => fn(t.UnwrapIfSingleException()), source, taskOptions, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                task.ContinueWith(t => fn(t.UnwrapIfSingleException()), taskOptions);
            }
            return task;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="fn"></param>
        /// <param name="onUiThread"></param>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        public static Task Error(this Task task, Action<Exception> fn,
            bool onUiThread = true,
            TaskContinuationOptions taskOptions = TaskContinuationOptions.OnlyOnFaulted)
        {
            if (onUiThread)
            {
                var source = new CancellationToken();
                task.ContinueWith(t => fn(t.UnwrapIfSingleException()), source, taskOptions, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                task.ContinueWith(t => fn(t.UnwrapIfSingleException()), taskOptions);
            }
            return task;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static Exception UnwrapIfSingleException<T>(this Task<T> task)
        {
            return task.Exception.UnwrapIfSingleException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static Exception UnwrapIfSingleException(this Task task)
        {
            return task.Exception.UnwrapIfSingleException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static Exception UnwrapIfSingleException(this Exception ex)
        {
            var aex = ex as AggregateException;
            if (aex == null)
                return ex;

            if (aex.InnerExceptions != null
                && aex.InnerExceptions.Count == 1)
                return aex.InnerExceptions[0].UnwrapIfSingleException();

            return aex;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static ConfiguredTaskAwaitable<TResult> AnyContext<TResult>(this Task<TResult> task)
        {
            return task.ConfigureAwait(continueOnCapturedContext: false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static ConfiguredTaskAwaitable AnyContext(this Task task)
        {
            return task.ConfigureAwait(continueOnCapturedContext: false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static ConfiguredTaskAwaitable<TResult> AnyContext<TResult>(this AwaitableDisposable<TResult> task) where TResult : IDisposable
        {
            return task.ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}