{
  pkgs,
  lib,
  buildDotnetModule,
  dotnetCorePackages,
  dockerTools,
  ...
}:
let
  fs = lib.fileset;
in
buildDotnetModule (finalAttrs: {
  pname = "ktisis";
  version = builtins.readFile ../version;

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
  dotnet-runtime = dotnetCorePackages.aspnetcore_9_0;

  executables = [ "Ktisis" ];

  meta = {
    license = lib.licenses.eupl12;
    mainProgram = "Ktisis";
    maintainers = with lib.maintainers; [ drakon64 ];
  };

  passthru.docker = dockerTools.buildLayeredImage {
    name = "ktisis";
    tag = "latest";

    config.Cmd = [ (lib.getExe finalAttrs.finalPackage) ];

    contents = [ dockerTools.caCertificates ];
  };
})
