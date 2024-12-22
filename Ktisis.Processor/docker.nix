{
  pkgs ?
    let
      npins = import ../npins;
    in
    import npins.nixpkgs { },
}:
let
  ktisis-processor = pkgs.callPackage ./. { };
in
pkgs.dockerTools.buildLayeredImage {
  name = "ktisis-processor";

  config.Entrypoint = [ "${ktisis-processor}/lib/ktisis-processor/Ktisis.Processor" ];

  contents = with pkgs; [ cacert ];

  tag = "latest";
}
