{
  pkgs ? import (import ./lon.nix).nixpkgs { },
}:
let
  ktisis = pkgs.callPackage ./src/package.nix { };
in
{
  inherit ktisis;

  docker = pkgs.dockerTools.buildLayeredImage {
    name = "ktisis";
    tag = "latest";

    config.Cmd = [ (pkgs.lib.getExe ktisis) ];
  };
}
