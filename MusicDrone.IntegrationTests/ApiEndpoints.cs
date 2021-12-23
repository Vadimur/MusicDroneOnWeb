namespace MusicDrone.IntegrationTests
{
    public static class ApiEndpoints
    {
        private const string _baseApi = "/api";
        public static class AccountEndpoints
        {
            private const string _controller = "/account";
            public static string Base = $"{_baseApi}{_controller}/";

            public static string Register = $"{Base}register";
            public static string Login = $"{Base}login";
            public static string Profile = $"{Base}profile";
        }

        public static class RoomEndpoints
        {
            private const string _controller = "/room";
            public static string Base = $"{_baseApi}{_controller}/";

            public static string Create = $"{Base}create";
            public static string GetAll = $"{Base}all";
            public static string WithId(string id) => $"{Base}{id}";
        }

        public static class RoomUserEndpoints
        {
            private const string _controller = "/roomuser";
            public static string Base = $"{_baseApi}{_controller}/";

            public static string EnterRoom = $"{Base}enter";
            public static string ExitRoom = $"{Base}exit";
            public static string UserRooms = $"{Base}rooms";
            public static string WithId(string id) => $"{Base}{id}";
        }
    }
}
