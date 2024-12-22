terraform {
  backend "gcs" {
    bucket = var.backend
  }

  required_providers {
    google = {
      source  = "hashicorp/google"
      version = "~> 6.12"
    }
  }
}

provider "google" {
  project = var.project
  region  = var.region
}
