locals {
  secrets = toset([
    "client-id",
    "private-key",
    "webhook-secret",
  ])
}

resource "google_project_service" "secret_manager" {
  service = "secretmanager.googleapis.com"
}

resource "google_secret_manager_secret" "secret" {
  for_each = local.secrets

  secret_id = each.value

  replication {
    auto {}
  }

  depends_on = [google_project_service.secret_manager]
}

resource "google_secret_manager_secret_iam_member" "ktisis_receiver" {
  for_each = local.secrets

  member    = google_service_account.ktisis_receiver.member
  role      = "roles/secretmanager.secretAccessor"
  secret_id = google_secret_manager_secret.secret[each.value].secret_id
}
