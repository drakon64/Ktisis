# Ktisis

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=drakon64_Ktisis&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=drakon64_Ktisis)

---

Ktisis is a pair of Google Cloud Run services for self-hosting GitHub Actions runners on Google Compute Engine.

## Building

Ktisis can be built with [Lix](https://lix.systems) (other Nix distributions should work but are unsupported).

Docker images can be built by running:

```shell
nix-build -A docker
```

Alternatively a Docker image can be built without Lix by running:

```shell
dotnet publish --os linux --arch x64 /t:PublishContainer
```

## License

EUPL v. 1.2 only.
