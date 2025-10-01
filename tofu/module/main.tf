terraform {
  required_providers {
    google = {
      source = "hashicorp/google"
    }
  }
}

locals {
  services = toset([
    "processor",
    "receiver",
    "runner",
  ])
}

data "google_project" "project" {}
