locals {
  secrets = toset([
    "github-webhook-secret",
  ])
}

resource "google_project_service" "secret_manager" {
  service = "secretmanager.googleapis.com"
}

resource "google_secret_manager_secret" "secret" {
  for_each = local.secrets

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

resource "google_secret_manager_secret_iam_member" "secret" {
  for_each = local.secrets

  member    = google_service_account.ktisis.member
  role      = "roles/secretmanager.secretAccessor"
  secret_id = google_secret_manager_secret.secret[each.key].secret_id
}
