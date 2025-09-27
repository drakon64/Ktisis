{
  pkgs ? import (import ./lon.nix).nixpkgs { },
}:
{
  ktisis = pkgs.callPackage ./src/package.nix { };
  docker = pkgs.callPackage ./src/docker.nix { };
}
