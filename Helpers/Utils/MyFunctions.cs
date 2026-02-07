using JobNexus.Common.Constant;
using Microsoft.AspNetCore.Identity;

namespace JobNexus.Helpers.Utils
{
    public static class MyFunctions
    {
        public static ErrorResponse ToErrorResponse(IEnumerable<IdentityError> errors)
        {
            var messages = errors.Select(e => e.Description).ToList();

            return new ErrorResponse(Error.ServerFailure, messages);
        }
    }
}
