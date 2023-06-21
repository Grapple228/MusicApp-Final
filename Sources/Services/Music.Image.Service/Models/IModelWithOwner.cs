using Music.Shared.Common;

namespace Music.Image.Service.Models;

public interface IModelWithOwner : IModel
{
    Guid OwnerId { get; set; }
}