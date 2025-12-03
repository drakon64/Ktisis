resource "google_project_service" "compute" {
  service = "compute.googleapis.com"
}

resource "google_compute_network" "network" {
  name = "ktisis"

  auto_create_subnetworks = false

  depends_on = [google_project_service.compute]
}

resource "google_compute_subnetwork" "subnetwork" {
  name    = "ktisis"
  network = google_compute_network.network.name

  ip_cidr_range = "192.168.0.0/16"
}

resource "google_compute_router" "router" {
  name    = "ktisis"
  network = google_compute_network.network.name
}

resource "google_compute_address" "nat" {
  name = "ktisis-nat-address"

  network_tier = "PREMIUM"

  depends_on = [google_project_service.compute]
}

resource "google_compute_router_nat" "nat" {
  name                               = "ktisis"
  router                             = google_compute_router.router.name
  source_subnetwork_ip_ranges_to_nat = "ALL_SUBNETWORKS_ALL_IP_RANGES"

  enable_dynamic_port_allocation = true
  nat_ip_allocate_option         = "MANUAL_ONLY"
  nat_ips                        = [google_compute_address.nat.self_link]
}

resource "google_compute_instance_template" "runner" {
  disk {
    disk_size_gb           = 14 + 4
    disk_type              = "hyperdisk-balanced"
    provisioned_iops       = 3000
    provisioned_throughput = 140
    source_image           = "projects/ubuntu-os-cloud/global/images/family/ubuntu-minimal-2404-lts-amd64"
  }

  machine_type = var.machine_type

  advanced_machine_features {
    enable_nested_virtualization = true
  }

  metadata = {
    enable-oslogin     = true
    enable-oslogin-2fa = true

    startup-script = templatefile("${path.module}/startup-script.sh.tftpl", {
      runner_url     = local.runner["browser_download_url"]
      runner_tarball = local.runner["name"]
    })
  }

  name = "ktisis"

  network_interface {
    network    = google_compute_network.network.self_link
    nic_type   = "GVNIC"
    subnetwork = google_compute_subnetwork.subnetwork.self_link
  }

  scheduling {
    instance_termination_action = "DELETE"

    max_run_duration {
      seconds = 6 * 60 * 60
    }
  }

  service_account {
    scopes = ["cloud-platform"]

    email = google_service_account.ktisis["runner"].email
  }

  shielded_instance_config {
    enable_integrity_monitoring = true
    enable_secure_boot          = true
    enable_vtpm                 = true
  }

  depends_on = [google_project_service.compute]
}

resource "google_service_account_iam_member" "runner" {
  member             = google_service_account.ktisis["processor"].member
  role               = "roles/iam.serviceAccountUser"
  service_account_id = google_service_account.ktisis["runner"].id
}

resource "google_compute_firewall" "iap" {
  name    = "allow-ingress-from-iap"
  network = google_compute_network.network.id

  allow {
    protocol = "tcp"

    ports = [22]
  }

  source_ranges = ["35.235.240.0/20"]
}
