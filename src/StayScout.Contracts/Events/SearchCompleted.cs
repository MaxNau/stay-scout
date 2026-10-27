namespace StayScout.Contracts.Events;

public sealed record SearchCompleted(string SearchId, DateTimeOffset CompletedAt);
