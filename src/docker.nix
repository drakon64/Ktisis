{
  pkgs,
  lib,
  callPackage,
  dockerTools,
  ...
}:
let
  ktisis = callPackage ./package.nix { };
in
dockerTools.buildLayeredImage {
  name = "ktisis";
  tag = "latest";

  config.Cmd = [ (lib.getExe ktisis) ];

  contents = [ dockerTools.caCertificates ];
}
