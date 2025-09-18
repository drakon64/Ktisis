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

  domain           = var.domain
  firestore_region = var.firestore_region
  region           = var.region
}
