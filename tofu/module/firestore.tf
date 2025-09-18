resource "google_project_service" "firestore" {
  service = "firestore.googleapis.com"
}

resource "google_firestore_database" "firestore" {
  location_id = var.region
  name        = "ktisis"
  type        = "FIRESTORE_NATIVE"

  depends_on = [google_project_service.firestore]
}
