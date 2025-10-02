resource "google_cloud_run_v2_service" "processor" {
  location = var.region
  name     = "ktisis-processor"

  ingress = "INGRESS_TRAFFIC_ALL"

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

      env {
        name = "KTISIS_GITHUB_CLIENT_ID"

        value_source {
          secret_key_ref {
            secret = google_secret_manager_secret.secret["github-client-id"].name

            version = "latest"
          }
        }
      }

      env {
        name = "KTISIS_GITHUB_PRIVATE_KEY"

        value_source {
          secret_key_ref {
            secret = google_secret_manager_secret.secret["github-private-key"].name

            version = "latest"
          }
        }
      }

      env {
        name = "KTISIS_PROJECT"

        value = data.google_project.project.project_id
      }

      env {
        name = "KTISIS_ZONES"

        value = join(" ", var.zones)
      }

      env {
        name = "KTISIS_SOURCE_INSTANCE_TEMPLATE"

        value = google_compute_instance_template.runner.id
      }

      resources {
        cpu_idle = true

        limits = {
          cpu    = "1000m"
          memory = "512Mi"
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

    max_instance_request_concurrency = 100

    scaling {
      max_instance_count = 1
      min_instance_count = 0
    }

    service_account = google_service_account.ktisis["processor"].email

    timeout = "60s"
    
    vpc_access {
      egress = "ALL_TRAFFIC"
      
      network_interfaces {
        network = google_compute_network.network.name
        subnetwork = google_compute_subnetwork.subnetwork.name
      }
    }
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
