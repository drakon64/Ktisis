output "receiver_uri" {
  value = var.built ? "${google_cloud_run_v2_service.ktisis_receiver[0].uri}/api/github/webhooks" : null
}
