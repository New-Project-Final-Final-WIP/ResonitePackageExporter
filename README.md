# ResonitePackageExporter
A Plugin for [Neos VR](https://neos.com/) that adds a ResonitePackage export option

This only allows exporting to Resonite, there is no conversion back into Neos

You can import Worlds into Resonite using the [ImportWorldResonitePackage mod](https://github.com/badhaloninja/ImportWorldResonitePackage)

## This is experimental, there is a chance for data loss when importing to resonite
The project is a little bit of a mess at the moment

The PostX script was taken from [XDelta's NeosFileStreamWriter project](https://github.com/XDelta/NeosFileStreamWriter/)


## Install
- Put the `ResonitePackageExporter.dll` and the `0Harmony.dll` in the `Neos/Libraries/` folder
- You need to load `ResonitePackageExporter.dll` with the `-LoadAssembly` launch argument, or use the `NeosLauncher.exe`
  - You do not need to load Harmony yourself, it will be automatically loaded by `ResonitePackageExporter`


## Usage
- When exporting an object/slot through the files tab, a new export option for `ResonitePackage` should be added to the top of the export dialogue, you can choose to export with or without variants included
- You can export a world by doing the same as above but holding a slot reference to the Root slot instead, then selecting `World Resonite Package`
- This plugin adds a `PackageExportable` component which can be used as any other `Exportable` component if you want
