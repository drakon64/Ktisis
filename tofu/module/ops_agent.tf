module "ops_agent_policy" {
  for_each = var.zones

  source  = "terraform-google-modules/cloud-operations/google//modules/ops-agent-policy"
  version = "0.6.0"
  
  project       = data.google_project.project.id
  zone          = each.value
  assignment_id = "ktisis-ops-agent-policy"

  agents_rule = {
    package_state = "installed"
    version       = "latest"
  }

  instance_filter = {
    all = true
  }
}
