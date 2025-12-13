namespace webStudioBlazor.ModelDTOs
{
    public class UserWithRolesDto
    {
        public string UserId { get; set; } = default!;
        public string Email { get; set; } = default!;
        public List<string> Roles { get; set; } = new();
    }
}
