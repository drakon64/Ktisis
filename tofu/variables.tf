variable "backend" {
  type = string
}

variable "built" {
  type    = bool
  default = false
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

variable "use_ssh_private_key" {
  type    = bool
  default = false
}
