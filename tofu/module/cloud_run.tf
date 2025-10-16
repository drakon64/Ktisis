locals {
  image = "${var.region}-docker.pkg.dev/${data.google_project.project.project_id}/${google_artifact_registry_repository.artifact_registry.name}/drakon64/ktisis@${data.docker_registry_image.ktisis.sha256_digest}"
}

resource "google_project_service" "cloud_run" {
  service = "run.googleapis.com"
}

data "docker_registry_image" "ktisis" {
  name = "ghcr.io/drakon64/ktisis:latest"
}
