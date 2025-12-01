# Description

This is a fork of official **Unity Visual Studio Code extension v1.2.3**, that allow to generate solution **(.sln)** and project **(.csproj)** files **WITHOUT** VS Code enabled as external script editor in Unity preferences.

This extension do **NOT** generate new **.slnx** solution file format, so it can solve problem of lacking of support .slnx files in vary IDE (such as Vim with using OmniSharp-Roslyn plugin)

# Installation

You must add following lines in your .../ProjectName/Packages/manifest.json

```diff
"dependencies": {
    ...
+   "kotpodlawkoy.sln.csproj.generator": "https://github.com/kotpodlawkoy/kotpodlawkoy.sln.csproj.generator.git#next/master",
    ...
  }
```
Yes, you have to do this everytime you create new project, but it isn't so bad, _I think_

# Usage

All of functionality are in Tools -> kot_pod_lawkoy | Generate -> ...

Settings are available in:
* Latter menu by pressing "Settings"
* Or Edit -> Preferences -> kot_pod_lawkoy | .sln Generator (button **"Generate .csproj"** does the same thing as **Tools -> kot_pod_lawkoy | Generate -> Generate**)
