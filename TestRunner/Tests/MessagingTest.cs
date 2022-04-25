using System.Diagnostics;
using TestRunner.Monitoring;

namespace TestRunner.Tests;

public class MessagingTest
{
    readonly Activate _activate;
    readonly Ping _ping;
    readonly ILogger<MessagingTest> _logger;

    public MessagingTest(Activate activate, Ping ping, ILogger<MessagingTest> logger)
    {
        _activate = activate;
        _ping = ping;
        _logger = logger;
    }

    public async Task RunTest(int parallelism, int durationInSeconds, CancellationToken cancel)
    {
        _logger.LogInformation("Starting messaging test with parallelism = {Parallelism}, duration = {Duration}s", parallelism,
            durationInSeconds);

        var actorIds = PrepareActorIds(parallelism);

        _logger.LogInformation("Activating {Parallelism} actors", parallelism);
        await ActivateActors(actorIds);

        _logger.LogInformation("Starting the messaging test");
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancel);
        cts.CancelAfter(TimeSpan.FromSeconds(durationInSeconds));

        var (totalMessages, testDuration) = await TestWorker(actorIds, cts.Token);

        _logger.LogInformation("Messaging test completed, total messages = {TotalMessages}, duration = {TestDuration}, Throughput = {Throughput:F2} msg/s",
            totalMessages, testDuration, totalMessages / testDuration.TotalSeconds);
    }

    string[] PrepareActorIds(int count) =>
        Enumerable.Range(1, count).Select(i => $"{Environment.MachineName}-{i}").ToArray();

    Task ActivateActors(string[] actorIds) =>
        Task.WhenAll(actorIds.Select(id => _activate(id)));

    async Task<(long TotalMessages, TimeSpan TestDuration)> TestWorker(string[] actorIds, CancellationToken cancel)
    {
        var totalMessages = 0L;
        var overallStopwatch = new Stopwatch();
        overallStopwatch.Start();

        var tasks = actorIds.Select(async id =>
        {
            var messageStopwatch = new Stopwatch(); 
            while (!cancel.IsCancellationRequested)
            {
                try
                {
                    messageStopwatch.Restart();
                    await _ping(id, Guid.NewGuid().ToString("N"));
                
                    TestMetrics.MessageLatency.Record(messageStopwatch.ElapsedTicks / (double)Stopwatch.Frequency);
                    TestMetrics.MessageCount.Add(1);
                    Interlocked.Increment(ref totalMessages);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error during test");
                    TestMetrics.ErrorCount.Add(1);
                }
            }

            messageStopwatch.Stop();
        });

        await Task.WhenAll(tasks);

        overallStopwatch.Stop();
        return (totalMessages, overallStopwatch.Elapsed);
    }
}