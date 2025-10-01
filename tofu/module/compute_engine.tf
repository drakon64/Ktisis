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

resource "google_compute_router_nat" "nat" {
  name                               = "ktisis"
  router                             = google_compute_router.router.name
  source_subnetwork_ip_ranges_to_nat = "ALL_SUBNETWORKS_ALL_IP_RANGES"

  nat_ip_allocate_option = "AUTO_ONLY"
}

resource "google_compute_instance_template" "runner" {
  machine_type = "n4-standard-4"

  disk {
    disk_size_gb = 14
    disk_type    = "pd-standard"
    source_image = "projects/ubuntu-os-cloud/global/images/family/ubuntu-minimal-2404-lts-amd64"
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
