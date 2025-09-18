resource "google_service_account" "ktisis" {
  account_id = "ktisis"
}

resource "google_project_iam_member" "compute" {
  member  = google_service_account.ktisis.member
  project = data.google_project.project.project_id
  role    = "roles/compute.instanceAdmin"
}
