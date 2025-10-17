resource "google_project_service" "cloud_run" {
  service = "run.googleapis.com"
}

data "google_artifact_registry_docker_image" "ktisis" {
  image_name    = "${var.use_ghcr ? "drakon64/" : ""}ktisis:latest"
  location      = var.region
  repository_id = google_artifact_registry_repository.artifact_registry.repository_id
}
