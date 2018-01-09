namespace Common.Contracts.User
{
    using System.Collections.Generic;

    public class GetUsersResultMessage: IGatewayResultMessage
    {
        public IEnumerable<UserDto> UserDtos { get; set; }
    }
}