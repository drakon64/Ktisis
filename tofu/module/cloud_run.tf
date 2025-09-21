resource "google_project_service" "cloud_run" {
  service = "run.googleapis.com"
}

data "google_artifact_registry_docker_image" "ktisis" {
  image_name    = "ktisis:latest"
  location      = var.region
  repository_id = google_artifact_registry_repository.artifact_registry.repository_id
}

resource "google_cloud_run_v2_service" "ktisis" {
  location = var.region
  name     = "ktisis"

  ingress = "INGRESS_TRAFFIC_INTERNAL_LOAD_BALANCER"

  # scaling {
  #   max_instance_count = 1
  # }

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

    scaling {
      max_instance_count = 1
    }

    service_account = google_service_account.ktisis.email

    timeout = "10s"
  }

  depends_on = [
    google_project_service.cloud_run,
    google_secret_manager_secret_iam_member.secret,
  ]
}

resource "google_cloud_run_v2_service_iam_member" "ktisis" {
  member = "allUsers"
  name   = google_cloud_run_v2_service.ktisis.name
  role   = "roles/run.invoker"
}
