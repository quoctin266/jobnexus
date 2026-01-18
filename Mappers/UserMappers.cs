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
                Age = appUser.Age,
                Address = appUser.Address,
                PhoneNumber = appUser.PhoneNumber ?? ""
            };
        }

    }
}
