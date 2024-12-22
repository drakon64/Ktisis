output "receiver_uri" {
  value = var.built ? "${google_cloud_run_v2_service.ktisis_receiver[0].uri}/api/github/webhooks" : null
}

output "processor_uri" {
  value = var.built ? google_cloud_run_v2_service.ktisis_processor[0].uri : null
}
