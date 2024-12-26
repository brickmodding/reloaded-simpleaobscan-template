/* RELOADED II AOB SCAN TEMPLATE
MADE BY DEVZ

This template uses LEGO Batman: The Videogame (2008, PC) as a base, but you should be able to use any executable.
*/

using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using reloaded.template.aobscan.Template;
using reloaded.template.aobscan.Configuration;

using Reloaded.Memory;
using Reloaded.Memory.Sigscan.Definitions.Structs;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Memory.Streams;
using System.Diagnostics;
using Reloaded.Memory.Interfaces;

namespace reloaded.template.aobscan;

public class Mod : ModBase
{
    private readonly IModLoader _modLoader;
    private readonly IReloadedHooks? _hooks;
    private readonly ILogger _logger;
    private readonly IMod _owner;
    private Config _configuration;
    private readonly IModConfig _modConfig;

    public Memory GameMem = Memory.Instance; // Game Memory
    private nuint BaseAdr = (nuint)Process.GetCurrentProcess().MainModule!.BaseAddress; // Finds base address of the current executable

    public Mod(ModContext context)
    {
        _modLoader = context.ModLoader;
        _hooks = context.Hooks;
        _logger = context.Logger;
        _owner = context.Owner;
        _configuration = context.Configuration;
        _modConfig = context.ModConfig;

        String[] patterns = // List of patterns to scan for
        [
            "68 48 01 00 00 68 ?? ?? ?? ?? 68 ?? ?? ?? ?? 68 ?? ?? ?? ??",
            "81 FF 47 01 00 00 ?? ?? ?? ?? ?? ?? ?? 8B 48 1C",
            "81 FF 47 01 00 00 ?? ?? ?? ?? ?? ?? 8B 0D ?? ?? ?? ?? 8B 51 1C",
            "81 FD 47 01 00 00 ?? ?? ?? ?? ?? ?? 8B 15 ?? ?? ?? ?? 8B 42 1C",
            "81 FE 47 01 00 00 ?? ?? ?? ?? ?? ?? 8B 15 ?? ?? ?? ?? 8B 42 1C",
            "81 FF 48 01 00 00 ?? ?? ?? ?? ?? ?? ?? 8B 50 1C",
        ];

        byte[][] new_bytes = // List of bytes that are written
        [
            [0x68, 0x0F, 0x27],
            [0x81, 0xFF, 0x0F, 0x27],
            [0x81, 0xFF, 0x0F, 0x27],
            [0x81, 0xFD, 0x0F, 0x27],
            [0x81, 0xFE, 0x0F, 0x27],
            [0x81, 0xFF, 0x0F, 0x27],
        ];

        for (int pi = 0; pi < patterns.Length; pi++) // Cycle through each pattern and patch bytes
        {
            ScanAndPatch(patterns[pi], new_bytes[pi]);
        }
    }

    private void PatchBytes(PatternScanResult result, byte[] bytes) // Writes bytes to memory
    {
        nint ScanAddress = result.Offset;
        _logger.WriteLine("Address: " + String.Format("{0:x}", ScanAddress)); // mainly for debug purposes

        GameMem.SafeWrite(BaseAdr + (nuint)ScanAddress, bytes);
    }

    private void ScanAndPatch(String aob, byte[] bytes) // scan for an array of bytes and patch write the given bytes
    {
        _modLoader.GetController<IStartupScanner>().TryGetTarget(out var startupScanner);

        startupScanner!.AddMainModuleScan(aob, result => PatchBytes(result, bytes));
    }

    #region Standard Overrides
    public override void ConfigurationUpdated(Config configuration)
    {
        _configuration = configuration;
        _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
    }
    #endregion

    #region For Exports, Serialization etc.
#pragma warning disable CS8618
    public Mod() { }
#pragma warning restore CS8618
    #endregion
}