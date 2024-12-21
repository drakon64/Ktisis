resource "google_project_service" "cloud_run" {
  service = "run.googleapis.com"
}

resource "google_cloud_run_v2_service" "ktisis" {
  count = var.built ? 1 : 0

  location = var.region
  name     = "ktisis"

  template {
    containers {
      image = "${var.region}-docker.pkg.dev/${var.common_project}/ktisis/ktisis:${var.tag}"

      env {
        name  = "PROJECT"
        value = var.project
      }

      env {
        name  = "REPOSITORY_OWNERS"
        value = "drakon64 lilyinstarlight"
      }

      env {
        name  = "ZONES"
        value = join(" ", var.zones)
      }

      env {
        name  = "COMPUTE_SERVICE_ACCOUNT"
        value = data.google_compute_default_service_account.compute.email
      }

      env {
        name = "GITHUB_WEBHOOK_SECRET"
        value_source {
          secret_key_ref {
            secret  = google_secret_manager_secret.secret["webhook-secret"].name
            version = "latest"
          }
        }
      }

      env {
        name = "GITHUB_CLIENT_ID"
        value_source {
          secret_key_ref {
            secret  = google_secret_manager_secret.secret["client-id"].name
            version = "latest"
          }
        }
      }

      env {
        name = "GITHUB_PRIVATE_KEY"
        value_source {
          secret_key_ref {
            secret  = google_secret_manager_secret.secret["private-key"].name
            version = "latest"
          }
        }
      }

      resources {
        cpu_idle = true

        limits = {
          cpu    = "1000m"
          memory = "128Mi"
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

    scaling {
      max_instance_count = 1
      min_instance_count = 0
    }

    session_affinity = true
    service_account  = google_service_account.ktisis.email
  }

  depends_on = [
    google_project_service.cloud_run,
    google_artifact_registry_repository_iam_member.ktisis,
    google_secret_manager_secret_iam_member.ktisis,
  ]
}
