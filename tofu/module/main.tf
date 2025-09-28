terraform {
  required_providers {
    google = {
      source = "hashicorp/google"
    }
  }
}

locals {
  services = toset([
    "receiver",
    "runner",
    "processor",
  ])
}

data "google_project" "project" {}
