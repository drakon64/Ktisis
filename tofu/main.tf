terraform {
  backend "gcs" {
    bucket = var.state_bucket

    prefix = "ktisis"
  }
}

provider "google" {
  project = var.project
  region  = var.region
}

module "ktisis" {
  source = "./module"

  domain = var.domain
  region = var.region
}
