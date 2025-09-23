resource "google_cloud_run_v2_service" "receiver" {
  location = var.region
  name     = "ktisis-receiver"

  ingress = "INGRESS_TRAFFIC_INTERNAL_LOAD_BALANCER"

  template {
    containers {
      image = data.google_artifact_registry_docker_image.ktisis.self_link

      env {
        name = "KTISIS_GITHUB_WEBHOOK_SECRET"

        value_source {
          secret_key_ref {
            secret = "github-webhook-secret"

            version = "latest"
          }
        }
      }

      env {
        name = "KTISIS_CLOUD_TASKS_QUEUE"

        value = google_cloud_tasks_queue.cloud_tasks.name
      }

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

    service_account = google_service_account.ktisis["receiver"].email

    timeout = "10s"
  }

  depends_on = [
    google_project_service.cloud_run,
    google_secret_manager_secret_iam_member.receiver_secret,
  ]
}

resource "google_cloud_run_v2_service_iam_member" "receiver" {
  member = "allUsers"
  name   = google_cloud_run_v2_service.receiver.name
  role   = "roles/run.invoker"
}
