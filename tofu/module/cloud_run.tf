locals {
  image = var.use_ghcr ? "${var.region}-docker.pkg.dev/${data.google_project.project.project_id}/${google_artifact_registry_repository.artifact_registry.name}/drakon64/ktisis@${data.docker_registry_image.ktisis[0].sha256_digest}" : data.google_artifact_registry_docker_image.ktisis[0].self_link
}

resource "google_project_service" "cloud_run" {
  service = "run.googleapis.com"
}

data "docker_registry_image" "ktisis" {
  count = var.use_ghcr ? 1 : 0
  
  name = "ghcr.io/drakon64/ktisis:latest"
}

data "google_artifact_registry_docker_image" "ktisis" {
  count = var.use_ghcr ? 0 : 1
  
  image_name    = "ktisis:latest"
  location      = var.region
  repository_id = google_artifact_registry_repository.artifact_registry.repository_id
}
