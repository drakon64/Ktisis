import Foundation

struct WorkflowJobQueued: Decodable {
  let action: Action
  let workflowJob: WorkflowJob
  let repository: Repository
}

enum Action: Decodable {
  case queued
}

struct WorkflowJob: Decodable {
  let id, runID: Int
  let labels: [String]
}

struct Repository: Decodable {
  let name: String
  let owner: Owner
}

struct Owner: Decodable {
  let login: String
}
