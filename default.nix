{
  pkgs ?
    let
      npins = import ./npins;
    in
    import npins.nixpkgs { },
}:
pkgs.buildDotnetModule {
  pname = "ktisis";
  version = "0.0.1";

  src = ./.;

  projectFile = "Ktisis.csproj";
  nugetDeps = ./deps.nix;

  dotnet-sdk = pkgs.dotnetCorePackages.sdk_9_0;
  dotnet-runtime = null;

  executables = [ ];

  selfContainedBuild = true;
}
