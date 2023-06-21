using Music.Shared.Common;

namespace Music.Shared.DTOs.Artists;

public record ArtistShort(Guid Id, string Name) : IModel;