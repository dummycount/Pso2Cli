# PSO2 Command Line Tools

This project provides command line tools for handling PSO2 data files. Use the `--help` argument for full usage information.

You will need the [.NET Framework 4.7.2 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472) to run the programs.

## aqp2fbx

Converts `.aqp` models to `.fbx` for importing into 3D modeling software.

### Examples

Read `pl_rbd_200000_bw.aqp` and `pl_rbd_200000_bw.aqn` and convert to `pl_rbd_200000_bw.fbx`:
```sh
aqp2fbx pl_rbd_200000_bw.aqp
```

Read `pl_rbd_200000_bw.aqp` and `my_skeleton.aqn` and convert to `my_model.fbx`:
```sh
aqp2fbx pl_rbd_200000_bw.aqp my_model.fbx -s my_skeleton.aqn
```

Include motion files when converting:
```sh
aqp2fbx pl_rbd_200000_bw.aqp -m np_std_rcf00_10010_shop_idle_lp.aqm -m np_std_rcf00_10011_shop_act_ed.aqm
```

## fbx2aqp

Converts `.fbx` files back to `.aqp`.

### Examples

Read `pl_rbd_200000_bw.fbx` and convert to `pl_rbd_200000_bw.aqp` and `pl_rbd_200000_bw.aqn` (does not overwrite the `.aqn` file if it already exists):
```sh
aqp2fbx pl_rbd_200000_bw.fbx
```

Same as above but do overwrite an existing `.aqn` file.
```sh
aqp2fbx pl_rbd_200000_bw.fbx --update-aqn
```

Read `pl_rbd_200000_bw.fbx` and convert to `my_model.aqp` and `my_model.aqn` (does not overwrite the `.aqn` file if it already exists):
```sh
aqp2fbx pl_rbd_200000_bw.fbx my_model.aqp
```

Read `pl_rbd_200000_bw.fbx` and convert to `my_model.aqp` and `my_model.aqn`:
```sh
aqp2fbx pl_rbd_200000_bw.fbx my_model.aqp  -s my_model.aqn
```

## dds2png

Converts `.dds` images to `.png` format.

### Examples

Convert `pl_rbd_200000_sk_d.dds` to `pl_rbd_200000_sk_d.png`:
```sh
dds2png pl_rbd_200000_sk_d.dds
```

Convert `pl_rbd_200000_sk_d.dds` to `diffuse.png`:
```sh
dds2png pl_rbd_200000_sk_d.dds diffuse.png
```

## ice

Creates and extracts ICE archives.

### Examples

List the files in the ICE archive `921bfc343f540292430b2932c74b0e98`:
```
ice list 921bfc343f540292430b2932c74b0e98
```

Same as above but output in JSON for reading by another program:
```
ice list 921bfc343f540292430b2932c74b0e98 --json
```

Extract the ICE archive `921bfc343f540292430b2932c74b0e98` to `921bfc343f540292430b2932c74b0e98.extracted/`:
```
ice unpack 921bfc343f540292430b2932c74b0e98
```

Same as above but do not organize into group1/group2 folders:
```
ice unpack 921bfc343f540292430b2932c74b0e98 --flatten
```

Extract, convert models to FBX format, and convert textures to PNG format:
```
ice unpack 921bfc343f540292430b2932c74b0e98 --fbx --png
```

Extract the ICE archive `921bfc343f540292430b2932c74b0e98` to `output/`:
```
ice unpack 921bfc343f540292430b2932c74b0e98 output
```

Pack the contents of the directory `921bfc343f540292430b2932c74b0e98.extracted/` to `921bfc343f540292430b2932c74b0e98`:
```
ice pack 921bfc343f540292430b2932c74b0e98.extracted
```

Pack the contents of the directory `output/` to `921bfc343f540292430b2932c74b0e98`:
```
ice pack output 921bfc343f540292430b2932c74b0e98
```

## Development

Requirements:

- [Visual Studio](https://visualstudio.microsoft.com/vs/community/) with the C# and C++ workflows installed.
- [Autodesk FBX SDK](https://www.autodesk.com/developer-network/platform-technologies/fbx-sdk-2020-2-1)
- [PowerShell](https://github.com/PowerShell/PowerShell/releases)

Open Pso2Cli.sln and build the solution. If the build fails, try it again a couple of times.

## Credits

Much of this code is based on [Aqua Model Tool](https://github.com/Shadowth117/PSO2-Aqua-Library) and [Zamboni](https://github.com/Shadowth117/Zamboni)