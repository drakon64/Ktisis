resource "google_project_service" "compute" {
  service = "compute.googleapis.com"
}

data "google_compute_default_service_account" "compute" {
  depends_on = [google_project_service.compute]
}

resource "google_service_account_iam_member" "compute" {
  member             = google_service_account.ktisis_processor.member
  role               = "roles/iam.serviceAccountUser"
  service_account_id = data.google_compute_default_service_account.compute.id
}
