namespace NexusGearMS.Helpers
{
    public static class SessionManager
    {
        public static int AccountID { get; set; }
        public static string Username { get; set; }
        public static int EmpID { get; set; }
        public static string EmpCode { get; set; }
        public static string FullName { get; set; }
        public static int RoleID { get; set; }
        public static string RoleName { get; set; }
        public static bool MustChangePwd { get; set; }

        public static void Clear()
        {
            AccountID = 0;
            Username = null;
            EmpID = 0;
            EmpCode = null;
            FullName = null;
            RoleID = 0;
            RoleName = null;
            MustChangePwd = false;
        }
    }
}
