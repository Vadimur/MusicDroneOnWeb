namespace MusicDrone.IntegrationTests
{
    public static class ApiEndpoints
    {
        private static string _base = "api";
        public static class AccountEndpoints
        {
            private static string _controller = "account";

            public static string Register = $"{_base}/{_controller}/register";
            public static string Login = $"{_base}/{_controller}/login";
            public static string Profile = $"{_base}/{_controller}/profile";
        }

        public static class RoomEndpoints
        {
            private static string _controller = "room";

            public static string Create = $"{_base}/{_controller}/createroom";
            public static string GetAll = $"{_base}/{_controller}/getallrooms";
            public static string GetSingle(string id) => $"{_base}/{_controller}/{id}";
            public static string Delete = $"{_base}/{_controller}/deleteroom";

        }
    }
}
