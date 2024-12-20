{
  pkgs ?
    let
      npins = import ./npins;
    in
    import npins.nixpkgs { },
}:
let
  ktisis = pkgs.callPackage ./. { };
in
pkgs.dockerTools.buildLayeredImage {
  name = "ktisis";

  config.Entrypoint = [ "${ktisis}/lib/ktisis/Ktisis" ];

  contents = with pkgs; [ cacert ];

  tag = "latest";
}
