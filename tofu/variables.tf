variable "allowed_repositories" {
  default = null
  type    = set(string)
}

variable "domain" {
  type = string
}

variable "instance_template" {
  type = object({
    disk = object({
      disk_size_gb = optional(number, 18),
      disk_type    = optional(string, "pd-balanced")
    })
  })
}

variable "machine_type" {
  default = "n4-standard-4"
}

variable "project" {
  type = string
}

variable "region" {
  type = string
}

variable "secret_replica_regions" {
  default = null
  type    = set(string)
}

variable "state_bucket" {
  type = string
}

variable "use_ghcr" {
  description = "Use pre-built Ktisis images from GitHub Packages"
  default     = true
}

variable "zones" {
  type = set(string)
}
