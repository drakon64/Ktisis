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

  retry_config {
    max_retry_duration = "${24 * 60 * 60}s"
  }

  depends_on = [google_project_service.cloud_tasks]
}

resource "google_cloud_tasks_queue_iam_member" "ktisis" {
  member = google_service_account.ktisis["receiver"].member
  name   = google_cloud_tasks_queue.cloud_tasks.name
  role   = "roles/cloudtasks.enqueuer"
}
