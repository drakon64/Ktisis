resource "google_project_service" "cloud_run" {
  service = "run.googleapis.com"
}

data "google_artifact_registry_docker_image" "ktisis" {
  image_name    = "ktisis"
  location      = var.region
  repository_id = google_artifact_registry_repository.artifact_registry.repository_id
}

resource "google_cloud_run_v2_service" "ktisis" {
  location = var.region
  name     = "ktisis"

  scaling {
    max_instance_count = 1
  }

  template {
    containers {
      image = data.google_artifact_registry_docker_image.ktisis.self_link

      resources {
        cpu_idle = true

        limits = {
          cpu    = "80m"
          memory = "128Mi"
        }
      }

      startup_probe {
        failure_threshold = 10
        period_seconds    = 1

        tcp_socket {
          port = 8080
        }
      }
    }

    scaling {
      max_instance_count = 1
    }

    timeout = "10s"
  }

  depends_on = [google_project_service.cloud_run]
}
