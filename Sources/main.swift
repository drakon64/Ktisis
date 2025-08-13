import ArgumentParser
import Hummingbird

@main
struct Ktisis: AsyncParsableCommand {
  // TODO: Replace with subcommand
  @Flag(help: "Run the Receiver service.")
  var receiver = false

  // TODO: Replace with subcommand
  @Flag(help: "Run the Processor job.")
  var processor = false

  @Option(name: .shortAndLong, help: "The GitHub organisations to allow requests from.")
  var organisations: [String] = []

  @Option(name: .shortAndLong, help: "The GitHub repositories to allow requests from.")
  var repositories: [String] = []

  mutating func run() async throws {
    if processor {

    } else if receiver {
      let router = Router().get { req, context in
        return "Hello, Swift!"
      }

      let app = Application(router: router)
      try await app.runService()
    }
  }
}
