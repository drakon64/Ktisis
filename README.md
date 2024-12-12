# Ktisis

Ktisis is a Google Cloud Run service and GitHub App for running ephemeral self-hosted Runners in GitHub Actions.

With Ktisis, a Runner instance is created when a job requests one, and the instance is terminated when the job is completed.

Ktisis is added to a GitHub Actions workflow like so:

```yaml
jobs:
  job-name:
    runs-on: [ linux, x64, self-hosted, ktisis ]
```

Alternatively an ARM64 runner can be specified with:

```yaml
jobs:
  job-name:
    runs-on: [ linux, ARM64, self-hosted, ktisis ]
```

It is also possible to use a custom machine family and type, and allocate a custom-size boot disk:

```yaml
jobs:
  job-name:
    runs-on: [ linux, x64, self-hosted, ktisis, ktisis-c3d-highcpu-4, ktisis-24GB ]
```

By default, Ktisis will use a machine type similar to that of the GitHub-hosted Runners (`n4-standard-4`/`c4a-standard-4`, 14GB boot disk).

## License

EUPL v. 1.2 only
