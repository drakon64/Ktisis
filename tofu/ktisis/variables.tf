variable "backend" {
  type = string
}

variable "branch" {
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

variable "repository_owner" {
  type = string
}

variable "zones" {
  type = list(string)
}
