namespace DynamoDb.Api.Dtos
{
    public record CustomerDto(Guid Id, string Name, string Address, string Email, DateTime? UpdatedAt);

}
