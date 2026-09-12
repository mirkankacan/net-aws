namespace DynamoDb.Api.Dtos
{
    public record UpdateCustomerDto(Guid Id, string Name, string Address, string Email, DateTime? ExpectedUpdatedAt);

}
