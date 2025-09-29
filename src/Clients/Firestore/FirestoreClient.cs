namespace Ktisis.Clients.Firestore;

internal static partial class FirestoreClient
{
    private static readonly string Url;

    static FirestoreClient()
    {
        var database =
            Environment.GetEnvironmentVariable("KTISIS_FIRESTORE_DATABASE")
            ?? throw new InvalidOperationException("KTISIS_FIRESTORE_DATABASE is null");

        Url = $"https://firestore.googleapis.com/v1/{database}";
    }
}
