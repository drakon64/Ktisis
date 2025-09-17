{
  pkgs ? import (import ./lon.nix).nixpkgs { },
}:
rec {
  ktisis =
    let
      fs = pkgs.lib.fileset;
    in
    pkgs.buildDotnetModule {
      pname = "ktisis";
      version = "0.0.1";

      src = fs.toSource {
        root = ./.;

        fileset = fs.difference (fs.gitTracked ./.) (
          fs.unions [
            ./default.nix
            ./shell.nix
            ./lon.lock
            ./lon.nix

            ./src/appsettings.Development.json
            ./src/deps.json

            ./.editorconfig
            ./.envrc
            ./.gitattributes
            ./.gitignore
            ./Ktisis.slnx
            ./LICENSE
            ./README.md

            ./.github
          ]
        );
      };

      projectFile = "src/Ktisis.csproj";
      nugetDeps = ./src/deps.json;

      dotnet-sdk = pkgs.dotnetCorePackages.sdk_9_0;
      dotnet-runtime = pkgs.dotnetCorePackages.aspnetcore_9_0;

      executables = [ "Ktisis" ];

      meta = {
        license = pkgs.lib.licenses.eupl12;
        mainProgram = "Ktisis";
        maintainers = with pkgs.lib.maintainers; [ drakon64 ];
      };
    };

  docker = pkgs.dockerTools.buildLayeredImage {
    name = "ktisis";
    tag = "latest";

    config.Cmd = [ (pkgs.lib.getExe ktisis) ];
  };
}
