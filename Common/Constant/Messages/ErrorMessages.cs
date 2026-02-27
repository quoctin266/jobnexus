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
        public const string EmailNotVerified = "Unverified email";

        // User Messages
        public const string UserNotFound = "User not found with provided id";
        public const string EmailNotFound = "User not found with provided email";

        // Company Employee Messages
        public const string EmployeeNotFound = "Employee not found with provided id";
        public const string InvalidEmployeeRole = "Cannot create employee with owner role";
        public const string DifferentCompany = "Cannot add employee to another company";
        public const string UserAlreadyEmployed = "User already belongs to a company or has a pending request";
        public const string ActiveEmploymentNotFound = "User doesn't have any active employment";
        public const string EmployeeNotInCompany = "Can not update status of an employee from another company";
        public const string SelfUpdateNotAllowed = "Can not update own status";

        // Company Messages
        public const string CompanyNotFound = "Company not found with provided id";

        // Company Request Messages
        public const string CompanyRequestNotFound = "Request not found with provided id";
        public const string CompanyRequestConflict = "Requests with status pending or approved already existed";
        public const string InvalidCompanyRequestStatusUpdate = "Can not update to status Pending";
        public const string CompanyRequestUpdateNotAllowed = "Can only update status of pending request";

        // Skill Messages
        public const string SkillNotFound = "Skill not found with provided id";
        public const string SkillInUse = "Cannot delete skill that is in use";

        // Job Messages
        public const string JobNotFound = "Job not found with provided id";
        public const string InvalidSalaryRange = "Max salary must be bigger than min salary";
        public const string InvalidDateRange = "End date can not be before start date";
        public const string InvalidDateValue = "End date and start date can not be in the past";
        public const string InvalidJobDuration = "Job duration must be at least 7 days";
        public const string InvalidJobStatus = "Can not update job status to pending";
        public const string JobClosed = "Job has already been closed";
        public const string JobUpdateNotAllowed = "Can not update approved or closed job";

        // Resume Messages
        public const string ResumeNotFound = "Resume not found with provided id";

        // Application Messages
        public const string ApplicationNotFound = "Application not found with provided id";
        public const string ApplicationNotAllowed = "Job currently does not accept application";
        public const string DuplicatedApplication = "User has already applied to this job";
        public const string ResumeNotOwned = "Can not use a resume not owned by applicant";
        public const string InvalidApplicationStatusUpdate = "Can not update to status Pending";
        public const string ApplicationUpdateNotAllowed = "Can only update status of pending application";
    }
}
