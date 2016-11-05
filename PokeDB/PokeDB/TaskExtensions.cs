using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PokeDB
{
    static class TaskExtensions
    {
        public static Task<T> ContinueWithLog<T>(this Task<T> task, string taskName = null, ExecutionOutcome when = ExecutionOutcome.Anyway)
        {
#if DEBUG
            var cont = WithLogBuildContinuation<Task<T>>(when, taskName ?? task.Id.ToString("\\#G"),
                id => t => Debug.WriteLine($"Task {id} is completed successfully:\n {t.Result}"));

            return task.ContinueWith(t =>
            {
                cont.Item1(t);
                return t;
            }, cont.Item2).Unwrap();
#endif // DEBUG
        }

        [Conditional("DEBUG")]
        public static void Log<T>(this Task<T> task, string taskName = null, ExecutionOutcome when = ExecutionOutcome.Anyway)
        {
            task.ContinueWithLog(taskName, when);
        }

        public static Task ContinueWithLog(this Task task, string taskName = null, ExecutionOutcome when = ExecutionOutcome.Anyway)
        {
#if DEBUG
            var cont = WithLogBuildContinuation<Task>(when, taskName ?? task.Id.ToString("\\##"), 
                id => t => Debug.WriteLine($"Task {id} is completed successfully."));

            return task.ContinueWith(cont.Item1, cont.Item2);
#endif // DEBUG
        }

        [Conditional("DEBUG")]
        public static void Log(this Task task, string taskName = null, ExecutionOutcome when = ExecutionOutcome.Anyway)
        {
            task.ContinueWithLog(taskName, when);
        }

#if DEBUG
        static Tuple<Action<T>, TaskContinuationOptions> WithLogBuildContinuation<T>(ExecutionOutcome when, string taskName, Func<string, Action<T>> onSuccessProvider)
            where T : Task
        {
            switch (when)
            {
                case ExecutionOutcome.OnError:
                    return Tuple.Create(new Action<T>(t => Debug.WriteLine($"Task {taskName} is failed:\n {t.Exception}.")), 
                        TaskContinuationOptions.OnlyOnFaulted);

                case ExecutionOutcome.OnSuccess:
                    return Tuple.Create(onSuccessProvider(taskName),
                        TaskContinuationOptions.OnlyOnRanToCompletion);

                default:
                    var onSuccess = onSuccessProvider(taskName);

                    return Tuple.Create(new Action<T>(t => {
                        switch (t.Status)
                        {
                            case TaskStatus.RanToCompletion:
                                onSuccess(t);
                                break;

                            case TaskStatus.Canceled:
                                Debug.WriteLine($"Task {taskName} is canceled.");
                                break;

                            case TaskStatus.Faulted:
                                Debug.WriteLine($"Task {taskName} is failed: {t.Exception}");
                                break;
                        }
                    }), TaskContinuationOptions.None);
            }
        }
#endif // DEBUG
    }
}
