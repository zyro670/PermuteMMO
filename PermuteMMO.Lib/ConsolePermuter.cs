using System.Diagnostics;
using PKHeX.Core;

namespace PermuteMMO.Lib;

/// <summary>
/// Logic to permute spawner data.
/// </summary>
public static class ConsolePermuter
{
    static ConsolePermuter() => Console.OutputEncoding = System.Text.Encoding.Unicode;

    /// <summary>
    /// Permutes all the areas to print out all possible spawns.
    /// </summary>
    public static (List<string>, List<string>) PermuteMassiveMassOutbreak(Span<byte> data)
    {
        List<string> alist = new();
        List<string> blist = new();
        string log = string.Empty;
        var block = new MassiveOutbreakSet8a(data);
        for (int i = 0; i < MassiveOutbreakSet8a.AreaCount; i++)
        {
            var area = block[i];
            var areaName = AreaUtil.AreaTable[area.AreaHash];
            if (!area.IsActive)
            {
                log += $"No outbreak in {areaName}.";
                continue;
            }
            Debug.Assert(area.IsValid);

            bool hasPrintedAreaMMO = false;
            for (int j = 0; j < MassiveOutbreakArea8a.SpawnerCount; j++)
            {
                var spawner = area[j];
                if (spawner.Status is MassiveOutbreakSpawnerStatus.None)
                    continue;

                Debug.Assert(spawner.HasBase);
                var seed = spawner.SpawnSeed;
                var spawn = new SpawnInfo
                {
                    BaseCount = spawner.BaseCount,
                    BaseTable = spawner.BaseTable,

                    BonusCount = spawner.BonusCount,
                    BonusTable = spawner.BonusTable,
                };

                var result = Permuter.Permute(spawn, seed);
                if (!result.HasResults)
                    continue;

                if (!hasPrintedAreaMMO)
                {
                    log += $"Found paths for Massive Mass Outbreaks in {areaName}.";
                    log += "==========";
                    hasPrintedAreaMMO = true;
                }

                log += $"Spawner {j+1} at ({spawner.X:F1}, {spawner.Y:F1}, {spawner.Z}) shows {SpeciesName.GetSpeciesName(spawner.DisplaySpecies, 2)}";
                log += $"{spawn}";
                blist.Add(result.PrintResults(spawner.DisplaySpecies));
            }

            if (!hasPrintedAreaMMO)
            {
                log += $"Found no results for any Massive Mass Outbreak in {areaName}.";
            }
            else
            {
                log += "Done permuting area.";
                log += "==========";
            }
            alist.Add(log);
        }
        return (alist, blist);
    }

    /// <summary>
    /// Permutes all the Mass Outbreaks to print out all possible spawns.
    /// </summary>
    public static (List<string>, List<string>) PermuteBlockMassOutbreak(Span<byte> data)
    {
        List<string> alist = new();
        List<string> blist = new();
        string log = string.Empty;
        log += "Permuting Mass Outbreaks.";
        var block = new MassOutbreakSet8a(data);
        for (int i = 0; i < MassOutbreakSet8a.AreaCount; i++)
        {
            var spawner = block[i];
            var areaName = AreaUtil.AreaTable[spawner.AreaHash];
            if (!spawner.HasOutbreak)
            {
                log += $"No outbreak in {areaName}.";
                continue;
            }
            Debug.Assert(spawner.IsValid);

            var seed = spawner.SpawnSeed;
            var spawn = new SpawnInfo
            {
                BaseCount = spawner.BaseCount,
                BaseTable = spawner.DisplaySpecies,
                Type = SpawnType.Outbreak,
            };

            var result = Permuter.Permute(spawn, seed);
            if (!result.HasResults)
            {
                log += $"Found no paths for {(Species)spawner.DisplaySpecies} Mass Outbreak in {areaName}.";
                continue;
            }

            log += $"Found paths for {(Species)spawner.DisplaySpecies} Mass Outbreak in {areaName}:";
            log += "==========";
            log += $"Spawner at ({spawner.X:F1}, {spawner.Y:F1}, {spawner.Z}) shows {SpeciesName.GetSpeciesName(spawner.DisplaySpecies, 2)}";
            log += $"{spawn}";
            blist.Add(result.PrintResults(spawner.DisplaySpecies));
        }
       log += "Done permuting Mass Outbreaks.";
       log += "==========";
        alist.Add(log);
        return (alist, blist);
    }

/// <summary>
/// Permutes a single spawn with simple info.
/// </summary>
public static string PermuteSingle(SpawnInfo spawn, ulong seed, ushort species)
    {
        string log = string.Empty;
        log += $"Permuting all possible paths for {seed:X16}.";
        log += $"Parameters: {spawn}";

        var result = Permuter.Permute(spawn, seed);
        log += result.PrintResults(species);

        log += "Done.";
        return log;
    }
}
