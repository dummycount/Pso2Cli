# PSO2 Command Line Tools

This project provides a command line tool for handling PSO2 data files. Use the `--help` argument for full usage information.

You will need the [.NET 8.0 Runtime](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks) to run the programs.

## CMX Command

Inspect character making index data

All commands support a `--bin` option to provide the path to the `pso2_bin` if it can't be determined automatically.

### pso cmx body

Print metadata for body parts in JSON format.

```sh
pso cmx body > body.json
```

### pso cmx colorchannels

Print color channel indices for body parts in JSON format.

```sh
pso cmx colorchannels > colorcannels.json
```

### pso cmx sheets

Generate file list spreadsheets in CSV format.

Write sheets to a `FileList` directory:
```sh
pso cmx sheets ./FileList
```

## Convert Command

Convert between file formats.

### pso convert aqp

Converts model files to PSO2 model format (`.aqp`). Currently, `.fbx` is the only supported input format.

Read `pl_rbd_200000_bw.fbx` and convert to `pl_rbd_200000_bw.aqp` and `pl_rbd_200000_bw.aqn` (does not overwrite the `.aqn` file if it already exists):
```sh
pso convert aqp pl_rbd_200000_bw.fbx
```

Same as above but do overwrite an existing `.aqn` file.
```sh
pso convert aqp pl_rbd_200000_bw.fbx --update-aqn
```

Read `pl_rbd_200000_bw.fbx` and convert to `my_model.aqp` and `my_model.aqn` (does not overwrite the `.aqn` file if it already exists):
```sh
pso convert aqp pl_rbd_200000_bw.fbx my_model.aqp
```

Read `pl_rbd_200000_bw.fbx` and convert to `my_model.aqp` and `my_model.aqn`:
```sh
pso convert aqp pl_rbd_200000_bw.fbx my_model.aqp --skeleton my_model.aqn
```

### pso convert fbx

Converts model files to `.fbx` for importing into 3D modeling software. Currently, `.aqp` is the only supported input format.

Read `pl_rbd_200000_bw.aqp` and `pl_rbd_200000_bw.aqn` and convert to `pl_rbd_200000_bw.fbx`:
```sh
pso2 convert fbx pl_rbd_200000_bw.aqp
```

Read `pl_rbd_200000_bw.aqp` and `my_skeleton.aqn` and convert to `my_model.fbx`:
```sh
pso2 convert fbx pl_rbd_200000_bw.aqp my_model.fbx --skeleton my_skeleton.aqn
```

Include motion files when converting:
```sh
pso2 convert fbx pl_rbd_200000_bw.aqp -m np_std_rcf00_10010_shop_idle_lp.aqm -m np_std_rcf00_10011_shop_act_ed.aqm
```

### pso convert png

Converts images to `.png` format. Currently, `.dds` is the only supported input format.

Convert `pl_rbd_200000_sk_d.dds` to `pl_rbd_200000_sk_d.png`:
```sh
pso convert png pl_rbd_200000_sk_d.dds
```

Convert `pl_rbd_200000_sk_d.dds` to `diffuse.png`:
```sh
pso convert png pl_rbd_200000_sk_d.dds diffuse.png
```

## ICE Command

Edit and inspect ICE archives

### pso ice list

Prints a list of the files contained in an ICE archive.

```sh
pso ice list 921bfc343f540292430b2932c74b0e98
```

### pso ice unpack

Extracts an ICE archive.

Extract the ICE archive `921bfc343f540292430b2932c74b0e98` to `921bfc343f540292430b2932c74b0e98.extracted/`:
```sh
pso ice unpack 921bfc343f540292430b2932c74b0e98
```

Same as above but organize into group1/group2 folders:
```sh
pso ice unpack 921bfc343f540292430b2932c74b0e98 --groups
```

Extract the ICE archive `921bfc343f540292430b2932c74b0e98` to `output/`:
```sh
pso ice unpack 921bfc343f540292430b2932c74b0e98 output
```

### pso ice pack

Create an ICE archive from a directory of files.

If files are contained in `group1` and `group2` directories, they will automatically be assigned groups. Otherwise, they are put in group 2 unless a list of files or file types is given with `--group1`/`-1`.

Pack the contents of the directory `921bfc343f540292430b2932c74b0e98.extracted/` to `921bfc343f540292430b2932c74b0e98`:
```sh
pso ice pack 921bfc343f540292430b2932c74b0e98.extracted
```

Pack the contents of the directory `output/` to `921bfc343f540292430b2932c74b0e98`:
```sh
pso ice pack output 921bfc343f540292430b2932c74b0e98
```

Pack the contents of the directory `921bfc343f540292430b2932c74b0e98.extracted/` to `921bfc343f540292430b2932c74b0e98` and do not include `.png` files:
```sh
pso ice pack 921bfc343f540292430b2932c74b0e98.extracted --ignore ".png"
```

Pack the contents of the directory `921bfc343f540292430b2932c74b0e98.extracted/` to `921bfc343f540292430b2932c74b0e98` and place `.acb` and `.snd` files in group 1:
```sh
pso ice pack 921bfc343f540292430b2932c74b0e98.extracted --group1 ".acb,.snd"
```

## Development

Requirements:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks)
- [Visual Studio](https://visualstudio.microsoft.com/vs/community/) with the C# and C++ workflows installed.
- [Autodesk FBX SDK](https://www.autodesk.com/content/dam/autodesk/www/adn/fbx/2020-1/fbx20201_fbxsdk_vs2017_win.exe)
- [PowerShell](https://github.com/PowerShell/PowerShell/releases)

First, clone the repo and set up its dependencies:

```sh
git clone https://github.com/dummycount/Pso2Cli.git --recurse-submodules
cd Pso2Cli
./SetupDependencies.ps1
```

Then you should be able to open Pso2Cli.sln and build the solution.

## Credits

Much of this code is based on [Aqua Model Tool](https://github.com/Shadowth117/PSO2-Aqua-Library) and [Zamboni](https://github.com/Shadowth117/Zamboni)
