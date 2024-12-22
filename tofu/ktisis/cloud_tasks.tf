resource "google_project_service" "cloud_tasks" {
  service = "cloudtasks.googleapis.com"
}

resource "google_cloud_tasks_queue" "cloud_tasks" {
  location = var.region

  name    = "ktisis-tasks-queue"
  project = data.google_project.project.project_id

  rate_limits {
    max_concurrent_dispatches = 100
    max_dispatches_per_second = 1
  }

  retry_config {
    max_attempts       = -1
    max_backoff        = "60s"
    max_retry_duration = "86400s"
    min_backoff        = "60s"
  }

  depends_on = [google_project_service.cloud_tasks]
}

resource "google_cloud_tasks_queue_iam_member" "ktisis" {
  member = google_service_account.ktisis_receiver.member
  name   = google_cloud_tasks_queue.cloud_tasks.name
  role   = "roles/cloudtasks.enqueuer"
}
