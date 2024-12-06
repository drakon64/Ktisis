resource "google_project_service" "iam_credentials" {
  service = "iamcredentials.googleapis.com"
}

resource "google_service_account" "github_actions" {
  account_id = "github-actions"

  display_name = "GitHub Actions"
}

resource "google_project_iam_member" "github_actions" {
  member  = google_service_account.github_actions.member
  project = data.google_project.project.id
  role    = "roles/owner"
}

resource "google_service_account" "ktisis" {
  account_id = "ktisis"

  display_name = "Ktisis"
}

resource "google_project_iam_member" "ktisis" {
  member  = google_service_account.ktisis.member
  project = data.google_project.project.id
  role    = "roles/compute.instanceAdmin"
}
