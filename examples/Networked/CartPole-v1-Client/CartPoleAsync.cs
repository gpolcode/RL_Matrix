using System;
using System.Threading.Tasks;
using Gym.Environments.Envs.Classic;
using Gym.Rendering.WinForm;
using OGameSim.Entities;
using OGameSim.Production;
using OneOf;
using RLMatrix;

public class CartPoleAsync : IEnvironmentAsync<float[]>
{
    public int stepCounter { get; set; }
    public int maxSteps { get; set; }
    public bool isDone { get; set; }
    public OneOf<int, (int, int)> stateSize { get; set; }
    public int[] actionSize { get; set; }

    private Player player;
    private float[] myState;

    public CartPoleAsync()
    {
        Task.Run(async () => await InitialiseAsync()).Wait(); // Initialize in constructor asynchronously
    }

    public Task<float[]> GetCurrentState()
    {
        if (isDone)
            Reset().Wait(); // Reset if done

        return Task.FromResult(myState ?? [.. Enumerable.Repeat(0f,614)]);
    }

    public async Task InitialiseAsync()
    {
        player = new();
        stepCounter = 0;
        maxSteps = 8000;
        stateSize = 614;
        actionSize = [63];
        await Task.Run(() => player = new());
        isDone = false;
        myState = new float[614];
    }

    public Task Reset()
    {
        return Task.Run(() =>
        {
            player = new(); // Assuming Reset is not async; wrap in Task.Run
            myState = [.. Enumerable.Repeat(0f,614)];
            isDone = false;
            stepCounter = 0;
        });
    }

    public Task<(float, bool)> Step(int[] actionsIds)
    {        
        return Task.Run(() =>
        {
            if(isDone)
                Reset().Wait(); // Reset if done

            var actionId = actionsIds[0];
            var reward = ApplyAction(player, actionId);
            myState = CalculateState();

            stepCounter++;
            if (stepCounter > maxSteps)
                isDone = true;

            if (isDone)
                reward = 0;

            return (reward, isDone);
        });
    }

    public static float ApplyAction(Player player, int action)
    {
        float Penalty()
        {
            // Console.WriteLine("Penalty");
            return -10; //Math.Min(-10, (float)player.Points / 100 * -1);
        }

        float TryUpgrade(IUpgradable upgradable)
        {
            var currentPoints = player.Points;

            if (player.TrySpendResources(upgradable.UpgradeCost))
            {
                // Console.WriteLine($"Upgrade {upgradable.GetType().Name} {upgradable.Level}");
                upgradable.Upgrade();
                return 200; //(float)(player.Points - currentPoints);
            }

            return Penalty();
        }

        float ProceedToNextDay()
        {
            // Console.WriteLine("Cash");
            player.ProceedToNextDay();
            return 100;
        }

        var planetIndex = (int)Math.Floor((action - 1) / 3d);
        if (planetIndex > player.Planets.Count - 1)
        {
            return Penalty();
        }

        var reward = (action, action % 3) switch
        {
            (0, _) => ProceedToNextDay(),
            (1, _) => TryUpgrade(player.Astrophysics),
            (2, _) => TryUpgrade(player.PlasmaTechnology),
            (_, 0) => TryUpgrade(player.Planets[planetIndex].MetalMine),
            (_, 1) => TryUpgrade(player.Planets[planetIndex].CrystalMine),
            (_, 2) => TryUpgrade(player.Planets[planetIndex].DeuteriumSynthesizer),
            _ => throw new NotImplementedException(),
        };

        return reward;
    }

    public float[] CalculateState()
    {
        var states = new List<float>();

        void AddResources(Resources resources)
        {
            states.Add(resources.Metal);
            states.Add(resources.Crystal);
            states.Add(resources.Deuterium);
        }

        void AddResourcesModifier(ResourcesModifier resourcesModifier)
        {
            states.Add((float)resourcesModifier.Metal);
            states.Add((float)resourcesModifier.Crystal);
            states.Add((float)resourcesModifier.Deuterium);
        }

        // Player
        AddResources(player.Resources);

        // Plasma
        states.Add(player.PlasmaTechnology.Level);
        AddResourcesModifier(player.PlasmaTechnology.Modifier);
        AddResources(player.PlasmaTechnology.UpgradeCost);

        // Astro
        states.Add(player.Astrophysics.Level);
        AddResources(player.Astrophysics.UpgradeCost);

        // Planets
        for (int i = 0; i < 20; i++)
        {
            if (i > player.Planets.Count - 1)
            {
                states.AddRange(Enumerable.Repeat(0f, 30));
                continue;
            }

            var planet = player.Planets[i];

            // Metal
            states.Add(planet.MetalMine.Level);
            AddResources(planet.MetalMine.UpgradeCost);
            AddResources(planet.MetalMine.TodaysProduction);
            AddResources(planet.MetalMine.UpgradeIncreasePerDay);

            // Crystal
            states.Add(planet.CrystalMine.Level);
            AddResources(planet.CrystalMine.UpgradeCost);
            AddResources(planet.CrystalMine.TodaysProduction);
            AddResources(planet.CrystalMine.UpgradeIncreasePerDay);

            // Deut
            states.Add(planet.DeuteriumSynthesizer.Level);
            AddResources(planet.DeuteriumSynthesizer.UpgradeCost);
            AddResources(planet.DeuteriumSynthesizer.TodaysProduction);
            AddResources(planet.DeuteriumSynthesizer.UpgradeIncreasePerDay);
        }

        if (states.Count != 614)
        {
            throw new NotImplementedException();
        }

        return states.ToArray();
    }
}
