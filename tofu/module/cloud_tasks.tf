resource "google_project_service" "cloud_tasks" {
  service = "cloudtasks.googleapis.com"
}

resource "google_cloud_tasks_queue" "cloud_tasks" {
  location = var.region

  name = "ktisis"

  rate_limits {
    max_concurrent_dispatches = 100
    max_dispatches_per_second = floor(80 / 60)
  }

  depends_on = [google_project_service.cloud_tasks]
}

resource "google_cloud_tasks_queue_iam_member" "ktisis" {
  for_each = toset([
    "enqueuer",
    "taskDeleter",
  ])

  member = google_service_account.ktisis.member
  name   = google_cloud_tasks_queue.cloud_tasks.name
  role   = "roles/cloudtasks.${each.value}"
}
