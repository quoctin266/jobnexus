namespace JobNexus.Common.Constant.Messages
{
    public static class ValidationMessages
    {
        // User Validation Messages
        public const string UsernameMaxLength = "Username can not exceed 20 characters";
        public const string EmailMaxLength = "Email can not exceed 20 characters";
        public const string DoBFormat = "DateOfBirth must be a valid ISO 8601 string";
        public const string GenderValue = "Invalid gender value";
        public const string AddressMaxLength = "Address can not exceed 250 characters";
        public const string PhoneNumberFormat = "Phone Number must be in 0xxxxxxxxx format";

        // Skill Validation Messages
        public const string SkillNameMaxLength = "Skill name can not exceed 20 characters";

        // Company Validation Messages
        public const string CompanyNameMaxLength = "Company name can not exceed 50 characters";
        public const string TINMaxLength = "TIN can not exceed 50 characters";

        // Company Employee Validation Messages
        public const string CompanyRoleValue = "Invalid company role value";

        // Company request Validation Messages
        public const string CompanyRequestStatus = "Invalid request status value";

        // Job Validation Messages
        public const string JobNameMaxLength = "Job name can not exceed 50 characters";
        public const string JobLocationMaxLength = "Job location can not exceed 20 characters";
        public const string JobLevelMaxLength = "Job level can not exceed 20 characters";
        public const string JobQuantityRange = "Job quantity must be in range 1 - 50";
        public const string JobSkillRange = "Job skills must be in range 1 - 10";
        public const string JobSalaryRange = "Job salary must be in range 1.000.000 - 100.000.000";
        public const string JobStatus = "Invalid job status value";

        // Resume Validation Messages
        public const string ResumeTitleMaxLength = "Resume title can not exceed 50 characters";
    }
}
