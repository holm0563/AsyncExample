using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncExample
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            // Anti-Pattern #1: Forgotten await.
            SleepAsync();
            var t = BoolAsync();
            Sleep();

            // Anti-Pattern #2: Ignoring async task
            LongRunningTask(); //Not so cleverly run this in the background and miss the exception

            // Anti-Pattern #3: Async void
            // asyncVoid(); //Also can miss the exception
            // return;

            // Anti-Pattern #4: Blocking calls
            var result = BoolAsync().Result; //Wait is the same for void methods
            var result2 = BoolAsync().GetAwaiter().GetResult(); //Same problem

            // Anti-Pattern #5: Missing ConfigureAwait(false)
            // The above code can cause deadlocks if any async method is not configured with ConfigureAwait(false)
            // In dot net core this is usually no longer the case, although this setting is still recommended when creating Nuget packages

            // Anti-Pattern #6: Mixing Foreach with async
            var list = new List<bool>{true};
            // list.ForEach(b=>LongRunningTask(b)); //not waiting for result
            // list.ForEach(async b=> await LongRunningTask(b)); //still wont always handle error if returned immediately
            // return;

            // Anti-Pattern #7: Excessive parallelization
            // array.ToDictionaryAsync, or 
            // See: https://github.com/holm0563/graphql-dotnet/commit/e8c7be7a2c6b2e9a12df76cfa1afa5eddd017bbf?branch=e8c7be7a2c6b2e9a12df76cfa1afa5eddd017bbf&diff=split

            // Anti-Pattern #8: Non thread safe side effects
            //for (var x = 0; x < 100; x++)
            //{
            //    await ProcessAsync((dynamic) list); //not a pure function
            //}

            await AntiPattern9();

        }

        public static Task AntiPattern9()
        {
            try
            {
                return LongRunningTask();
            }
            catch (Exception ex)
            {
                // will not get executed.
                throw new Exception("Test");
            }
        }

        public static async Task SleepAsync()
        {
            await Task.Run(() => { Thread.Sleep(10); });
        }

        public static async Task<bool> BoolAsync()
        {
            return await Task.Run(() =>
            {
                Thread.Sleep(10);
                return false;
            }).ConfigureAwait(false);
        }

        public static async Task LongRunningTask(bool result = false)
        {
            await Task.Run(() => throw new ArgumentNullException());
        }

        public static async void AsyncVoid()
        {
            await Task.Run(() => throw new ArgumentNullException());
        }

        public static async Task<bool> BoolWithError()
        {
            await Task.Run(() => throw new ArgumentNullException());

            return false;
        }

        public static async Task ProcessAsync(List<dynamic> results)
        {
            await Task.Run(() => Thread.Sleep(50));

            // do other stuff 
            Thread.Sleep(50);

            results.Add(true);
        }


        public static void Sleep()
        {
            Thread.Sleep(10);
        }
    }
}