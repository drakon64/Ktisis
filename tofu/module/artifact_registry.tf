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

  mode = "REMOTE_REPOSITORY"

  remote_repository_config {
    docker_repository {
      custom_repository {
        uri = "https://ghcr.io"
      }
    }
  }
  
  vulnerability_scanning_config {
    enablement_config = "DISABLED"
  }

  depends_on = [google_project_service.artifact_registry]
}
