resource "google_project_service" "firestore" {
  service = "firestore.googleapis.com"
}

resource "google_firestore_database" "firestore" {
  location_id = coalesce(var.firestore_region, var.region)
  name        = "ktisis"
  type        = "FIRESTORE_NATIVE"

  deletion_policy = "DELETE"

  depends_on = [google_project_service.firestore]
}

resource "google_project_iam_member" "firestore" {
  member  = google_service_account.ktisis.member
  project = data.google_project.project.project_id
  role    = "roles/datastore.user"
}
