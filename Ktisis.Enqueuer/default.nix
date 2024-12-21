{
  pkgs ?
    let
      npins = import ../npins;
    in
    import npins.nixpkgs { },
}:
let
  fs = pkgs.lib.fileset;
  sourceFiles = fs.unions [
    (fs.fileFilter (file: file.hasExt "cs" || file.hasExt "csproj") ./.)
    ./appsettings.json
    ./Properties/launchSettings.json

    (fs.fileFilter (file: file.hasExt "cs" || file.hasExt "csproj") ../Ktisis.Common)
  ];
in
pkgs.buildDotnetModule {
  pname = "ktisis-enqueuer";
  version = "0.0.1";

  src = fs.toSource {
    fileset = sourceFiles;
    root = ../.;
  };

  projectFile = "Ktisis.Enqueuer/Ktisis.Enqueuer.csproj";
  #nugetDeps = ./deps.nix;

  dotnet-sdk = pkgs.dotnetCorePackages.sdk_9_0;
  dotnet-runtime = null;

  executables = [ ];

  selfContainedBuild = true;
}
