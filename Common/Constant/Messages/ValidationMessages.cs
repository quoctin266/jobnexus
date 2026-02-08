namespace JobNexus.Common.Constant.Messages
{
    public static class ValidationMessages
    {
        public const string UsernameMaxLength = "Username can not exceed 20 characters";
        public const string EmailMaxLength = "Email can not exceed 20 characters";
        public const string DoBFormat = "DateOfBirth must be a valid ISO 8601 string";
        public const string GenderValue = "Invalid gender value";
        public const string AddressMaxLength = "Address can not exceed 250 characters";
        public const string PhoneNumberFormat = "Phone Number must be in 0xxxxxxxxx format";

        public const string SkillNameMaxLength = "Skill name can not exceed 20 characters";

        public const string CompanyNameMaxLength = "Company name can not exceed 50 characters";
        public const string TINMaxLength = "TIN can not exceed 50 characters";

        public const string CompanyRoleValue = "Invalid company role value";

        public const string CompanyRequestStatus = "Invalid request status value";
    }
}
