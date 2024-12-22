resource "google_cloud_run_v2_service" "ktisis_processor" {
  count = var.built ? 1 : 0

  ingress  = "INGRESS_TRAFFIC_INTERNAL_ONLY"
  location = var.region
  name     = "ktisis-processor"

  template {
    containers {
      image = "${var.region}-docker.pkg.dev/${var.common_project}/ktisis/ktisis-processor:${var.tag}"

      env {
        name  = "PROJECT"
        value = data.google_project.project.project_id
      }

      env {
        name  = "REGION"
        value = var.region
      }
      
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
    service_account  = google_service_account.ktisis_processor.email
  }

  depends_on = [
    google_project_service.cloud_run,
    google_artifact_registry_repository_iam_member.ktisis,
  ]
}
