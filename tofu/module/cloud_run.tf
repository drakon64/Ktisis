locals {
  image = "ghcr.io/drakon64/ktisis:latest"
}

resource "google_project_service" "cloud_run" {
  service = "run.googleapis.com"
}
