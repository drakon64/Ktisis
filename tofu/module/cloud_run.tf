resource "google_project_service" "cloud_run" {
  service = "run.googleapis.com"
}

locals {
  # TODO: Get latest image hash
  image = "${var.region}-docker.pkg.dev/${data.google_project.project.project_id}/${google_artifact_registry_repository.artifact_registry.name}/drakon64/ktisis:latest"
}
