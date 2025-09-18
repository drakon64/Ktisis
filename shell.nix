{
  pkgs ? import (import ./lon.nix).nixpkgs { },
}:
pkgs.mkShellNoCC {
  packages = with pkgs; [
    graphviz
    lon
    nixfmt-rfc-style
    opentofu
  ];
}
