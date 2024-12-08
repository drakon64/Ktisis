data "google_service_account" "github_actions" {
  account_id = "github-actions"

  project = var.common_project
}

resource "google_project_iam_member" "github_actions" {
  member  = data.google_service_account.github_actions.member
  project = data.google_project.project.id
  role    = "roles/run.developer"
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
