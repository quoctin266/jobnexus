using JobNexus.Dtos.User;
using JobNexus.Models;

namespace JobNexus.Mappers
{
    public static class UserMappers
    {
        public static UserDto ToUserDto(this AppUser appUser)
        {
            return new UserDto
            {
                Id = appUser.Id,
                Email = appUser.Email ?? "",
                Username = appUser.UserName ?? "",
                DateOfBirth = appUser.DateOfBirth,
                Gender = appUser.Gender,
                Address = appUser.Address,
                PhoneNumber = appUser.PhoneNumber ?? ""
            };
        }

        public static UserSummaryDto ToUserSummaryDto(this AppUser appUser)
        {
            return new UserSummaryDto
            {
                Id = appUser.Id,
                Email = appUser.Email ?? "",
                Username = appUser.UserName ?? "",
            };
        }
    }
}
