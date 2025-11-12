{
  pkgs ? import (import ./lon.nix).nixpkgs { },
}:
pkgs.mkShellNoCC {
  packages = with pkgs; [
    dotnetCorePackages.sdk_10_0
    graphviz
    lon
    nixfmt-rfc-style
    nix-eval-jobs
    opentofu
  ];

  passthru = {
    lon = pkgs.mkShellNoCC {
      packages = [ pkgs.lon ];
    };

    nix-eval-jobs = pkgs.mkShellNoCC {
      packages = [ pkgs.nix-eval-jobs ];
    };

    opentofu = pkgs.mkShellNoCC {
      packages = [ pkgs.opentofu ];
    };
  };
}
