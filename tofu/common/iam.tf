resource "google_project_service" "iam_credentials" {
  service = "iamcredentials.googleapis.com"
}

resource "google_service_account" "github_actions" {
  account_id = "github-actions"

  display_name = "GitHub Actions"
}
