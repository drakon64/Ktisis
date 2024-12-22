resource "google_cloud_run_v2_service" "ktisis_processor" {
  count = var.built ? 1 : 0

  location = var.region
  name     = "ktisis-processor"

  template {
    containers {
      image = "${var.region}-docker.pkg.dev/${var.common_project}/ktisis/ktisis-processor:${var.tag}"

      resources {
        cpu_idle = true

        limits = {
          cpu    = "1000m"
          memory = "512Mi"
        }
      }

      startup_probe {
        failure_threshold = 9

        http_get {
          path = "/"
        }

        initial_delay_seconds = 1
        period_seconds        = 1
        timeout_seconds       = 1
      }
    }

    max_instance_request_concurrency = 100

    scaling {
      max_instance_count = 1
      min_instance_count = 0
    }

    session_affinity = true
    service_account  = google_service_account.ktisis_receiver.email
  }

  depends_on = [
    google_project_service.cloud_run,
    google_artifact_registry_repository_iam_member.ktisis,
    google_secret_manager_secret_iam_member.ktisis_receiver,
  ]
}
