{
  pkgs ?
    let
      npins = import ../npins;
    in
    import npins.nixpkgs { },
}:
let
  ktisis-receiver = pkgs.callPackage ./. { };
in
pkgs.dockerTools.buildLayeredImage {
  name = "ktisis-receiver";

  config.Entrypoint = [ "${ktisis-receiver}/lib/ktisis-receiver/Ktisis.Receiver" ];

  contents = with pkgs; [ cacert ];

  tag = "1.0.0";
}
