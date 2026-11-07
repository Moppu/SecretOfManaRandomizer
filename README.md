# Secret of Mana Randomizer
It's a multi-mode randomizer for the SNES game Secret of Mana.

Features:
- Open world randomizer - the current most popular mode. Grants Flammie Drum early, and randomly distributes spells, mana seeds, characters, etc. to various locations.
- Vanilla randomizer - randomly swaps enemies, bosses, spells, etc. for a full vanilla run
- Ancient cave - older mode that procedurally generates maps.  Needs work!
- Boss rush - older mode where you fight all bosses in a random order.  Needs work!
- Chaos - older mode where you proceed through randomly-linked vanilla maps.  Needs work!

- For all modes, various optional quality of life improvements and bugfixes.

# Building
Check out and load the .sln in Visual Studio.  I've been using VS 2017.  Anything later than that should theoretically also work.

I'll be posting releases, so unless you're actively working on the project, you shouldn't need to build it.

## macOS with Mono (full UI)
The regular `SoMRandomizer.exe` can run under Mono with an X server. This has been tested on macOS with MacPorts, and the script should also work on Linux, though that is untested. Install `sudo port install mono libgdiplus xorg-server dotnet-sdk-8` and start X11 (`open -a /Applications/MacPorts/X11.app`). Build the exe with `dotnet build -c Release SoMRandomizer`, or use one from a release folder after copying `run-mono.sh` next to it. Then run the following from the repository root.

```sh
./run-mono.sh                              # opens the UI
./run-mono.sh path/to/SoMRandomizer.exe    # explicit exe location
```

On macOS the script sets `MONO_MWF_MAC_FORCE_X11=1` and `MONO_WINFORMS_XIM_STYLE=disabled`, which work around two Mono crashes. The script documents both. Input method composition is unavailable as a result. In UI mode the script runs from the exe's folder, as double-clicking the exe on Windows does, so logs are written there and a `music` folder next to the exe is found. Passing randomizer arguments to the script runs the command-line mode described below under Mono instead.

If button labels are missing and dropdowns are drawn several times too tall, the installed libgdiplus reports inflated font metrics. This affects MacPorts libgdiplus 6.0.5_3 and older, which are built with Pango newer than 1.43. The fix is upstream libgdiplus commit [39e85126dd](https://github.com/mono/libgdiplus/commit/39e85126dd), included in 6.1, and is proposed for MacPorts in [macports/macports-ports#36447](https://github.com/macports/macports-ports/pull/36447).

## macOS and Linux with .NET 8 (command line only)
The `SoMRandomizer.Cli` project builds the open world generator against .NET 8 so that it runs natively on macOS and Linux. It is intentionally not part of `SoMRandomizer.sln`, because Visual Studio 2017 cannot load .NET 8 projects.

Install the .NET 8 SDK. With MacPorts, run `sudo port install dotnet-sdk-8`. Then run the following from the repository root.

```sh
dotnet build -c Release SoMRandomizer.Cli
dotnet SoMRandomizer.Cli/bin/Release/net8.0/SoMRandomizer.Cli.dll \
  srcRom="Secret of Mana (USA).sfc" dstRom=out.sfc seed=MySeed \
  options="opEnemies=swap opBosses=random"
```

All four arguments are required. The `options` value is a space-separated list of `key=value` pairs using the property names in `SoMRandomizer/config/settings`, and omitted options take their defaults. The destination file is overwritten without confirmation. The exit code is 0 on success, 1 for missing or malformed arguments, and 2 if generation fails. Spoiler and generation logs are written to the current directory, and a music pack, if present, is read from a `music` folder in the current directory.

# Using
Instructions will be here eventually for how to use the rando.  For now join the discord and ask if you have questions.

Releases will be posted here (no music pack)

and also here: https://secretofmanaancientcave.blogspot.com/ (with music pack)

# Contributing
Blerp

# Discord
https://discord.gg/YfmUHqU

We run a weekly open world seed, and this is currently the main source of general information, questions, etc. for the randomizer.
