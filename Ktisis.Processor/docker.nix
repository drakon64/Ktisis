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

  config = {
    Entrypoint = [ "${ktisis-processor}/lib/ktisis-processor/Ktisis.Processor" ];

    Labels = {
      "org.opencontainers.image.source" = "https://github.com/drakon64/Ktisis";
      "org.opencontainers.image.licenses" = "EUPL-1.2";
    };
  };

  contents = with pkgs; [ cacert ];

  tag = "1.0.0";
}
