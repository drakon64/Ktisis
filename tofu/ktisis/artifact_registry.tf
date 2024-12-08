data "google_artifact_registry_repository" "artifact_registry" {
  location      = var.region
  repository_id = "ktisis"

  project = var.common_project
}

resource "google_artifact_registry_repository_iam_member" "ktisis" {
  member     = "serviceAccount:service-${data.google_project.project.number}@serverless-robot-prod.iam.gserviceaccount.com"
  repository = data.google_artifact_registry_repository.artifact_registry.id
  role       = "roles/artifactregistry.reader"
}
