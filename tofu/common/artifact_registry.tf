resource "google_project_service" "artifact_registry" {
  service = "artifactregistry.googleapis.com"
}

resource "google_artifact_registry_repository" "artifact_registry" {
  format        = "DOCKER"
  repository_id = "ktisis"

  cleanup_policies {
    id = "Delete untagged"

    action = "DELETE"

    condition {
      tag_state = "UNTAGGED"
    }
  }

  depends_on = [google_project_service.artifact_registry]
}

resource "google_artifact_registry_repository_iam_member" "github_actions" {
  member     = google_service_account.github_actions.member
  repository = google_artifact_registry_repository.artifact_registry.id
  role       = "roles/artifactregistry.writer"
}
