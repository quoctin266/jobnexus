using JobNexus.Extensions;
using JobNexus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace JobNexus.Helpers.Authorization
{
    public class CompanyRequestOwnerHandler : AuthorizationHandler<OperationAuthorizationRequirement, CompanyRequest>
    {
        protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        CompanyRequest resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            if (requirement.Name != Operations.Read.Name &&
                requirement.Name != Operations.Update.Name)
            {
                return Task.CompletedTask;
            }

            var userId = context.User.GetUserId();

            if (userId == null)
                return Task.CompletedTask;

            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
            }

            if (resource.AppUserId == userId)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
