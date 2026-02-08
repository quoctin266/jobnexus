using JobNexus.Common.Constant;
using JobNexus.Common.Constant.Messages;
using JobNexus.Dtos.User;
using JobNexus.Helpers.Authorization;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces.BusinessService;
using JobNexus.Interfaces.Repository;
using JobNexus.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace JobNexus.Services.Business
{
    public class UserService : IUserService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAuthorizationService _authorizationService;

        public UserService(IAccountRepository accountRepository, IAuthorizationService authorizationService)
        {
            _accountRepository = accountRepository;
            _authorizationService = authorizationService;
        }

        public async Task<ServiceResult<AppUser>> GetById(string id)
        {
            var user = await _accountRepository.GetByIdAsync(id);
            if (user == null)
                return ServiceResult<AppUser>.Failure(StatusCodes.Status404NotFound, 
                                                      Error.NotFound, [ErrorMessages.UserNotFound]);

            return ServiceResult<AppUser>.Success(user);
        }

        public async Task<ServiceResult<AppUser>> Update(string id, UpdateUserDto updateUserDto, ClaimsPrincipal userClaims)
        {
            var user = await _accountRepository.GetByIdAsync(id);

            if(user is null) 
                return ServiceResult<AppUser>.Failure(StatusCodes.Status404NotFound, 
                                                      Error.NotFound, [ErrorMessages.UserNotFound]);

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(userClaims, user, Operations.Update);

            if (!authorizationResult.Succeeded)
                return ServiceResult<AppUser>.Failure(StatusCodes.Status403Forbidden, 
                                                      Error.Forbidden, [ErrorMessages.NoPermission]);

            await _accountRepository.UpdateUserAsync(user, updateUserDto);

            return ServiceResult<AppUser>.Success(user);
        }

        public async Task<ServiceResult<VoidType>> Delete(string id)
        {
            var user = await _accountRepository.GetByIdAsync(id);

            if (user is null)
                return ServiceResult<VoidType>.Failure(StatusCodes.Status404NotFound,
                                                      Error.NotFound, [ErrorMessages.UserNotFound]);

            var result = await _accountRepository.DeleteAsync(user);

            if (!result.Succeeded)
                return ServiceResult<VoidType>.Failure(StatusCodes.Status500InternalServerError,
                                                      Error.ServerFailure, [ErrorMessages.ServerError]);

            return ServiceResult<VoidType>.Success(new VoidType());
        }
    }
}
