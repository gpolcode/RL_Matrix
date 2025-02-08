
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RLMatrix;
using RLMatrix.Agents.Common;

public class TrainingHostedService : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {        
        var opts = new DQNAgentOptions() {
            Depth = 4
        };

        const string saveDirectory = @"C:/Users/Elsahr/Downloads/agent_storage";

        var envs = Enumerable.Repeat(0, 25).Select(x => new CartPole()).ToList();
        var myAgent = new LocalDiscreteRolloutAgent<float[]>(opts, envs);

        var newestSaveDirectory = new DirectoryInfo(saveDirectory)
            .GetDirectories()
            .OrderByDescending(d => d.LastWriteTimeUtc).FirstOrDefault();

        if (newestSaveDirectory != null)
        {
            await myAgent.Load(newestSaveDirectory.ToString());
        }

        while (true)
        {
            var identifier = DateTime.Now.ToString("yyyy'_'MM'_'dd'_'HH'_'mm'_'ss");

            for (int i = 0; i < 8000 * 3; i++)
            {
                await myAgent.Step();
            }

            var dir = Path.Combine(saveDirectory, identifier);
            Directory.CreateDirectory(dir);
            await myAgent.Save(dir);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
