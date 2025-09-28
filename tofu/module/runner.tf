locals {
  runner = one(
    [for url in jsondecode(data.http.runner.response_body)["assets"] : url if strcontains(url["name"], "linux-x64")]
  )
}

data "http" "runner" {
  url = "https://api.github.com/repos/actions/runner/releases/latest"

  request_headers = {
    Accept               = "application/vnd.github+json"
    X-GitHub-Api-Version = "2022-11-28"
  }
}
