terraform {
  required_providers {
    google = {
      source = "hashicorp/google"
    }

    http = {
      source  = "hashicorp/http"
      version = "~> 3"
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
