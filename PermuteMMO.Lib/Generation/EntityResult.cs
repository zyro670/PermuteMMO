using PKHeX.Core;

namespace PermuteMMO.Lib;

/// <summary>
/// Spawned Pokémon Data that can be encountered.
/// </summary>
public sealed class EntityResult
{
    public string Name { get; init; } = string.Empty;
    public readonly byte[] IVs = { byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue };

    public ulong GroupSeed { get; init; }
    public int Index { get; init; }
    public ulong SlotSeed { get; init; }
    public ulong GenSeed { get; init; }
    public int Level { get; init; }

    public uint EC { get; set; }
    public uint FakeTID { get; set; }
    public uint PID { get; set; }

    public uint ShinyXor { get; set; }
    public int RollCountUsed { get; set; }
    public int RollCountAllowed { get; set; }
    public ushort Species { get; init; }
    public ushort Form { get; init; }

    public bool IsShiny { get; set; }
    public bool IsAlpha { get; init; }
    public byte Ability { get; set; }
    public byte Gender { get; set; }
    public byte Nature { get; set; }
    public byte Height { get; set; }
    public byte Weight { get; set; }

    public bool IsOblivious => BehaviorUtil.Oblivious.Contains(Species);
    public bool IsSkittish => BehaviorUtil.Skittish.Contains(Species);
    public bool IsAggressive => IsAlpha || !(IsSkittish || IsOblivious);

    public string GetSummary()
    {
        var shiny = IsShiny ? $"{(ShinyXor == 0 ? '■' : '★')} - Shiny Rolls:  {RollCountUsed,2}" : "";
        var alpha = IsAlpha ? "αlpha - " : "NOT αlpha - ";
        var gender = Gender switch
        {
            2 => "",
            1 => " (F)",
            _ => " (M)",
        };
        return $"{alpha}{Name}\nShiny: {shiny}\nPID: {PID:X8}\nEC: {EC:X8}\nIVs: {IVs[0]}/{IVs[1]}/{IVs[2]}/{IVs[3]}/{IVs[4]}/{IVs[5]}\nGender: {gender}\nLevel: {Level}\nNature: {(Nature)Nature}\n";

    }    
}
