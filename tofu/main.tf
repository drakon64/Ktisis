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

  allowed_repositories   = var.allowed_repositories
  domain                 = var.domain
  machine_type           = var.machine_type
  region                 = var.region
  secret_replica_regions = var.secret_replica_regions
  use_ghcr               = var.use_ghcr
  zones                  = var.zones
}
