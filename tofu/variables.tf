variable "allowed_repositories" {
  default = null
  type    = set(string)
}

variable "domain" {
  type = string
}

variable "firestore_region" {
  default = null
  type    = string
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

variable "zones" {
  type = set(string)
}
