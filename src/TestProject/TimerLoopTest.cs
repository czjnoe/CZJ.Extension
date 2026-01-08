using System.Threading.Tasks;

namespace TestProject
{
    [TestClass]
    public sealed class TimerLoopTest
    {
        [TestMethod]

        public async Task SimpleTimerLoopTest()
        {

            var timerLoop = new TimerLoop(async () =>
            {
                await Task.Delay(500);
                Console.WriteLine($"执行逻辑");
                await Task.CompletedTask;
            }, options: new TimerLoopOptions
            {
                Interval = TimeSpan.FromSeconds(2),
                MaxExecutionCount = 5,
                OnExecuted = context =>
                {
                    Console.WriteLine($"Executed at {DateTime.Now}, Execution Count: {context.ExecutionCount}");
                },
                OnError = context =>
                {
                    Console.WriteLine($"Error occurred: {context.Exception?.Message}");
                },
                OnStopped = count =>
                {
                    Console.WriteLine($"Timer loop stopped after {count} executions.");
                }
            });
            try
            {
                await timerLoop.StartAsync();
                Task.Delay(10 * 1000).Wait();
                Assert.AreEqual(5, timerLoop.ExecutionCount);
            }
            finally
            {
                await timerLoop.StopAsync();
                timerLoop.Dispose();
            }
        }

        public async Task TimerLoopManagerTest()
        {
            using var manager = new TimerLoopManager();

            manager.Register("task1", async ct =>
            {
                Console.WriteLine($"[Task1 {DateTime.Now:HH:mm:ss}] 执行中...");
                await Task.Delay(500, ct);
            }, new TimerLoopOptions
            {
                Interval = TimeSpan.FromSeconds(2)
            });

            manager.Register("task2", async ct =>
            {
                Console.WriteLine($"[Task2 {DateTime.Now:HH:mm:ss}] 执行中...");
                await Task.Delay(300, ct);
            }, new TimerLoopOptions
            {
                Interval = TimeSpan.FromSeconds(1.5)
            });

            await manager.StartAllAsync();
            await Task.Delay(TimeSpan.FromSeconds(8));
            await manager.StopAllAsync();
        }
    }
}
