resource "google_service_account" "ktisis" {
  for_each = local.services

  account_id = "ktisis-${each.key}"
}

resource "google_project_iam_member" "compute" {
  member  = google_service_account.ktisis["processor"].member
  project = data.google_project.project.project_id
  role    = "roles/compute.instanceAdmin"
}
