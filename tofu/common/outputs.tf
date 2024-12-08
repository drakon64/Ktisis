output "artifact_registry" {
  value = "${var.region}-docker.pkg.dev/${var.project}/ktisis"
}

output "service_account" {
  value = google_service_account.github_actions.email
}

output "workload_identity_provider" {
  value = google_iam_workload_identity_pool_provider.github_actions.name
}
