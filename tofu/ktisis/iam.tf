data "google_service_account" "github_actions" {
  account_id = "github-actions"

  project = var.common_project
}

resource "google_project_iam_member" "github_actions" {
  member  = data.google_service_account.github_actions.member
  project = data.google_project.project.id
  role    = "roles/run.developer"
}

resource "google_service_account" "ktisis_receiver" {
  account_id = "ktisis-receiver"

  display_name = "Ktisis Receiver"
}

resource "google_service_account" "ktisis_processor" {
  account_id = "ktisis-processor"

  display_name = "Ktisis Processor"
}

resource "google_service_account_iam_member" "ktisis_receiver" {
  member             = data.google_service_account.github_actions.member
  role               = "roles/iam.serviceAccountUser"
  service_account_id = google_service_account.ktisis_receiver.id
}

resource "google_service_account_iam_member" "ktisis_processor" {
  member             = data.google_service_account.github_actions.member
  role               = "roles/iam.serviceAccountUser"
  service_account_id = google_service_account.ktisis_processor.id
}

resource "google_project_iam_member" "ktisis_processor" {
  member  = google_service_account.ktisis_processor.member
  project = data.google_project.project.id
  role    = "roles/compute.instanceAdmin"
}
