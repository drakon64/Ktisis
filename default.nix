{
  pkgs ? import (import ./lon.nix).nixpkgs { },
}:
rec {
  ktisis = pkgs.buildDotnetModule {
    pname = "ktisis";
    version = "0.0.1";

    src = ./.;

    projectFile = "Ktisis.csproj";
    nugetDeps = ./deps.json;

    dotnet-sdk = pkgs.dotnetCorePackages.sdk_9_0;
    dotnet-runtime = pkgs.dotnetCorePackages.runtime_9_0;

    executables = [ "Ktisis" ];
  };

  docker = pkgs.dockerTools.buildLayeredImage {
    name = "ktisis";
    tag = "latest";

    contents = [ ktisis ];

    config.Cmd = [ (pkgs.lib.getExe ktisis) ];
  };
}
