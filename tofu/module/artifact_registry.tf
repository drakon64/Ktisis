resource "google_project_service" "artifact_registry" {
  service = "artifactregistry.googleapis.com"
}

resource "google_artifact_registry_repository" "artifact_registry" {
  format        = "DOCKER"
  repository_id = "ktisis"

  cleanup_policies {
    id = "Untagged"

    action = "DELETE"

    condition {
      tag_state = "UNTAGGED"
    }
  }

  depends_on = [google_project_service.artifact_registry]
}
