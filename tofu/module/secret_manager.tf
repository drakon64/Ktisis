resource "google_project_service" "secret_manager" {
  service = "secretmanager.googleapis.com"
}

resource "google_secret_manager_secret" "secret" {
  for_each = toset([
    "github-webhook-secret",
  ])

  secret_id = each.key

  replication {
    user_managed {
      dynamic "replicas" {
        for_each = coalesce(var.secret_replica_regions, toset([var.region]))

        content {
          location = replicas.key
        }
      }
    }
  }

  depends_on = [google_project_service.secret_manager]
}
