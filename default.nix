{
  pkgs ? import (import ./lon.nix).nixpkgs { },
}:
rec {
  ktisis =
    let
      fs = pkgs.lib.fileset;

      sourceFiles = fs.difference ./. (
        fs.unions [
          (fs.maybeMissing ./result)
          ./default.nix
          ./shell.nix
          ./deps.json
          ./lon.lock
          ./lon.nix
        ]
      );
    in
    pkgs.buildDotnetModule {
      pname = "ktisis";
      version = "0.0.1";

      src = fs.toSource {
        root = ./.;
        fileset = sourceFiles;
      };

      projectFile = "Ktisis.csproj";
      nugetDeps = ./deps.json;

      dotnet-sdk = pkgs.dotnetCorePackages.sdk_9_0;
      dotnet-runtime = pkgs.dotnetCorePackages.aspnetcore_9_0;

      executables = [ "Ktisis" ];
    };

  docker = pkgs.dockerTools.buildLayeredImage {
    name = "ktisis";
    tag = "latest";

    config.Cmd = [ (pkgs.lib.getExe ktisis) ];
  };
}
