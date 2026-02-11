namespace JobNexus.Common.Constant.Messages
{
    public static class ErrorMessages
    {
        // General Messages
        public const string NoPermission = "Cannot access this resource";
        public const string InvalidToken = "Missing/Invalid Token";
        public const string ServerError = "Internal server error";

        // Authentication Messages
        public const string InvalidCredentials = "Invalid email/password";

        // User Messages
        public const string UserNotFound = "User not found with provided id";

        // Company Employee Messages
        public const string EmployeeNotFound = "Employee not found with provided id";
        public const string InvalidEmployeeRole = "Cannot create employee with owner role";
        public const string DifferentCompany = "Cannot add employee to another company";
        public const string UserAlreadyEmployed = "User already belongs to a company or has a pending request";
        public const string ActiveEmploymentNotFound = "User doesn't have any active employment";

        // Company Messages
        public const string CompanyNotFound = "Company not found with provided id";

        // Company Request Messages
        public const string CompanyRequestNotFound = "Request not found with provided id";
        public const string CompanyRequestConflict = "Requests with status pending or approved already existed";
        public const string InvalidCompanyRequestStatusUpdate = "Can not update to status Pending";

        // Skill Messages
        public const string SkillNotFound = "Skill not found with provided id";
        public const string SkillInUse = "Cannot delete skill that is in use";

        // Job Messages
        public const string JobNotFound = "Job not found with provided id";
        public const string InvalidSalaryRange = "Max salary must be bigger than min salary";
        public const string InvalidDateRange = "End date can not be before start date";
        public const string InvalidDateValue = "End date and start date can not be in the past";
        public const string InvalidJobDuration = "Job duration must be at least 7 days";
    }
}
