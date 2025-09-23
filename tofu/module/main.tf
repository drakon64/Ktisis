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
    "processor",
  ])
}

data "google_project" "project" {}

resource "google_project_service" "compute" {
  service = "compute.googleapis.com"
}
