namespace Project.Constants;

public static class ErrorMessages
{
    public static class Connection
    {
        public const string ConnectionFailed = "коннекшн не удался";
    }

    public static class Podcast
    {
        public const string NotFoundById = "Podcast с id {0} не найден";
        public const string NotFoundByIdRu = "Подкаст с id {0} не найден";
    }

    public static class Episode
    {
        public const string NotFoundById = "Эпизод с id {0} не найден";
    }

    public static class User
    {
        public const string NotFoundById = "Пользователь с id {0} не найден";
    }

    public static class Playlist
    {
        public const string NotFoundById = "Плейлист с id {0} не найден";
        public const string NotFoundEpisodeOrPlaylist = "Плейлист с id {0} или эпизод с id {1} не найден, либо эпизод уже добавлен";
        public const string EpisodeNotFoundInPlaylist = "Эпизод с id {0} не найден в плейлисте с id {1}";
    }

    public static class Category
    {
        public const string NotFoundById = "Категория с id {0} не найден";
    }

    public static class Validation
    {
        public const string SearchParameterEmpty = "Параметр поиска не может быть пустым";
        public const string IdMismatch = "ID в URL не совпадает с ID в теле запроса";
    }
}

