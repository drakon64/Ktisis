resource "google_cloud_run_v2_service" "processor" {
  location = var.region
  name     = "ktisis-processor"

  ingress = "INGRESS_TRAFFIC_INTERNAL_ONLY"

  template {
    containers {
      image = data.google_artifact_registry_docker_image.ktisis.self_link

      dynamic "env" {
        for_each = var.allowed_repositories != null ? var.allowed_repositories : []

        content {
          name = "KTISIS_GITHUB_REPOSITORIES"

          value = join(" ", var.allowed_repositories)
        }
      }

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

    service_account = google_service_account.ktisis["processor"].email

    timeout = "60s"
  }

  depends_on = [
    google_project_service.cloud_run,
    google_secret_manager_secret_iam_member.processor_secret,
  ]
}

resource "google_cloud_run_v2_service_iam_member" "processor" {
  member = google_service_account.ktisis["receiver"].member
  name   = google_cloud_run_v2_service.processor.name
  role   = "roles/run.invoker"
}
