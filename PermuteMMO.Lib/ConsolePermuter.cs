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
    public static (List<Species>, List<string>, List<string>) PermuteMassiveMassOutbreak(Span<byte> data)
    {
        List<Species> alist = new();
        List<string> blist = new();
        List<string> clist = new();
        string log = string.Empty;
        var block = new MassiveOutbreakSet8a(data);
        for (int i = 0; i < MassiveOutbreakSet8a.AreaCount; i++)
        {
            var area = block[i];
            var areaName = AreaUtil.AreaTable[area.AreaHash];
            if (!area.IsActive)
            {
                log += $"\nNo outbreak in {areaName}.";
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
                var seed = spawner.GroupSeed;
                var spawn = new SpawnInfo(spawner);

                var result = Permuter.Permute(spawn, seed);
                if (!result.HasResults)
                    continue;

                if (!hasPrintedAreaMMO)
                {
                    log += $"\nFound paths for Massive Mass Outbreaks in {areaName}.";
                    log += "\n==========";
                    hasPrintedAreaMMO = true;
                }

                log += $"\nSpawner {j+1} at ({spawner.X:F1}, {spawner.Y:F1}, {spawner.Z}) shows {SpeciesName.GetSpeciesName(spawner.DisplaySpecies, 2)}";
                log += spawn.GetSummary($"\nParameters: \nSeed{seed}");
                var lines = result.GetLines();
                foreach (var line in lines)
                    blist.Add(line);
                //blist.Add(result.PrintResults(spawner.DisplaySpecies));
                //blist.Add(result.PrintResults(hasSkittish));
                alist.Add((Species)spawner.DisplaySpecies);
            }

            if (!hasPrintedAreaMMO)
            {
                log += $"\nFound no results for any Massive Mass Outbreak in {areaName}.";
            }
            else
            {
                log += "\nDone permuting area.";
                log += "\n==========";
            }
            clist.Add(log);
        }
        return (alist, blist, clist);
    }

    /// <summary>
    /// Permutes all the Mass Outbreaks to print out all possible spawns.
    /// </summary>
    public static (List<Species>, List<string>, List<string>) PermuteBlockMassOutbreak(Span<byte> data)
    {
        List<Species> alist = new();
        List<string> blist = new();
        List<string> clist = new();
        string log = string.Empty;
        log += "Permuting Mass Outbreaks.";
        var block = new MassOutbreakSet8a(data);
        for (int i = 0; i < MassOutbreakSet8a.AreaCount; i++)
        {
            var spawner = block[i];
            var areaName = AreaUtil.AreaTable[spawner.AreaHash];
            if (!spawner.HasOutbreak)
            {
                log += $"\nNo outbreak in {areaName}.";
                continue;
            }
            Debug.Assert(spawner.IsValid);

            var seed = spawner.GroupSeed;
            var spawn = new SpawnInfo(spawner);
            var result = Permuter.Permute(spawn, seed);
            if (!result.HasResults)
            {
                log += $"\nFound no paths for {(Species)spawner.DisplaySpecies} Mass Outbreak in {areaName}.";
                continue;
            }

            log += $"\nFound paths for {(Species)spawner.DisplaySpecies} Mass Outbreak in {areaName}:";
            log += "\n==========";
            log += $"\nSpawner at ({spawner.X:F1}, {spawner.Y:F1}, {spawner.Z}) shows {SpeciesName.GetSpeciesName(spawner.DisplaySpecies, 2)}";
            log += spawn.GetSummary($"\nParameters: {spawn}\nSeed{seed}");
            var lines = result.GetLines();
            foreach (var line in lines)
                blist.Add(line);
            //blist.Add(result.PrintResults(spawner.DisplaySpecies));
            //blist.Add(result.PrintResults(hasSkittish));
            alist.Add((Species)spawner.DisplaySpecies);
        }
       log += "\nDone permuting Mass Outbreaks.";
       log += "\n==========";
        clist.Add(log);
        return (alist, blist, clist);
    }

    /// <summary>
    /// Permutes a single spawn with simple info.
    /// </summary>
    public static List<string> PermuteSingle(SpawnInfo spawn, ulong seed, ushort species)
    {
        List<string> log = new();
        log.Add($"\nPermuting all possible paths for {seed:X16}.");
        log.Add($"\nBase Species: {SpeciesName.GetSpeciesName(species, 2)}");
        log.Add(spawn.GetSummary($"\nParameters: {spawn}\nSeed{seed}"));

        var result = Permuter.Permute(spawn, seed);
        if (!result.HasResults)
            log.Add("\nNo results found. Try another outbreak! :(");
        else
        {
            var lines = result.GetLines();
            foreach (var line in lines)
                log.Add("\n"+line);
        }

        log.Add("\nDone.");
        return log;
    }
}
