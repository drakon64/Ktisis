{
  pkgs ? import (import ./lon.nix).nixpkgs { },
}:
{
  ktisis = pkgs.callPackage ./src/package.nix { };
  docker = pkgs.pkgsCross.gnu64.callPackage ./src/docker.nix { };
}
