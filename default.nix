{
  pkgs ? import (import ./lon.nix).nixpkgs { },
}:
rec {
  ktisis = pkgs.callPackage ./src/package.nix { };

  docker = pkgs.dockerTools.buildLayeredImage {
    name = "ktisis";
    tag = "latest";

    config.Cmd = [ (pkgs.lib.getExe ktisis) ];
  };
}
