let
  pkgs = import (import ./lon.nix).nixpkgs { };
in
pkgs.mkShellNoCC {
  packages = with pkgs; [
    lon
    nixfmt-rfc-style
  ];
}
