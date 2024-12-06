output "artifact_registry" {
  value = "${var.region}-docker.pkg.dev/${var.project}/ktisis"
}

output "receiver_uri" {
  value = var.built ? "${google_cloud_run_v2_service.ktisis[0].uri}/api/github/webhooks" : null
}

output "service_account" {
  value = google_service_account.github_actions.email
}

output "workload_identity_provider" {
  value = google_iam_workload_identity_pool_provider.github_actions.name
}
