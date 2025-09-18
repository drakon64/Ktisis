terraform {
  required_providers {
    google = {
      source = "hashicorp/google"
    }
  }
}

data "google_project" "project" {}

resource "google_project_service" "compute" {
  service = "compute.googleapis.com"
}
