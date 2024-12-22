variable "backend" {
  type = string
}

variable "built" {
  type    = bool
  default = false
}

variable "common_project" {
  type = string
}

variable "project" {
  type = string
}

variable "region" {
  type = string
}

variable "repository_owners" {
  type = list(string)
}

variable "tag" {
  type    = string
  default = "latest"
}

variable "zones" {
  type = list(string)
}
