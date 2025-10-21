variable "allowed_repositories" {
  default = null
  type    = set(string)
}

variable "domain" {
  type = string
}

variable "machine_type" {
  default = "n2d-standard-4"
}

variable "region" {
  type = string
}

variable "secret_replica_regions" {
  default = null
  type    = set(string)
}

variable "use_ghcr" {
  description = "Use pre-built Ktisis images from GitHub Packages"
  default     = true
}

variable "zones" {
  type = set(string)
}
