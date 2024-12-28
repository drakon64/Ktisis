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

  config = {
    Entrypoint = [ "${ktisis-receiver}/lib/ktisis-receiver/Ktisis.Receiver" ];

    Labels = {
      "org.opencontainers.image.source" = "https://github.com/drakon64/Ktisis";
      "org.opencontainers.image.licenses" = "EUPL-1.2";
    };
  };

  contents = with pkgs; [ cacert ];

  tag = "1.0.0";
}
