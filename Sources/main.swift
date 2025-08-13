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
