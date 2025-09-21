{
  pkgs,
  lib,
  buildDotnetModule,
  dotnetCorePackages,
  ...
}:
let
  fs = lib.fileset;
in
buildDotnetModule {
  pname = "ktisis";
  version = "0.0.1";

  src = fs.toSource {
    root = ./.;

    fileset = fs.difference (./.) (
      fs.unions [
        ./appsettings.Development.json
        (lib.fileset.maybeMissing ./bin)
        (lib.fileset.maybeMissing ./config)
        (lib.fileset.maybeMissing ./obj)
      
        (lib.fileset.maybeMissing ./deps.json)
        ./package.nix
      ]
    );
  };

  projectFile = "Ktisis.csproj";
  nugetDeps = ./deps.json;

  dotnet-sdk = dotnetCorePackages.sdk_9_0;
  dotnet-runtime = null;

  executables = [ "Ktisis" ];

  selfContainedBuild = true;

  meta = {
    license = lib.licenses.eupl12;
    mainProgram = "Ktisis";
    maintainers = with lib.maintainers; [ drakon64 ];
  };
}
