resource "google_service_account" "ktisis" {
  for_each = local.services

  account_id = "ktisis-${each.key}"
}

resource "google_service_account_iam_member" "iam" {
  for_each = toset([
    "processor",
    "receiver",
  ])

  member             = google_service_account.ktisis[each.value].member
  role               = "roles/iam.serviceAccountUser"
  service_account_id = google_service_account.ktisis[each.value].id
}

resource "google_project_iam_member" "compute" {
  member  = google_service_account.ktisis["processor"].member
  project = data.google_project.project.project_id
  role    = "roles/compute.instanceAdmin"
}
