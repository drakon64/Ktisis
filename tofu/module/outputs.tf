output "load_balancer_ips" {
  value = {
    v4 = module.lb-http.external_ip
    v6 = module.lb-http.external_ipv6_address
  }
}

output "nat_ip" {
  value = google_compute_address.nat.address
}
