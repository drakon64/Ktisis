{
  pkgs ?
    let
      npins = import ../npins;
    in
    import npins.nixpkgs { },
}:
let
  ktisis-enqueuer = pkgs.callPackage ./. { };
in
pkgs.dockerTools.buildLayeredImage {
  name = "ktisis-enqueuer";

  config.Entrypoint = [ "${ktisis-enqueuer}/lib/ktisis-enqueuer/Ktisis.Enqueuer" ];

  contents = with pkgs; [ cacert ];

  tag = "latest";
}
