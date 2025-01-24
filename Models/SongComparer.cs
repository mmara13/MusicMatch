using MusicMatch.Models;

public class SongComparer : IEqualityComparer<Song>
{
    public bool Equals(Song x, Song y)
    {
        return x?.Id == y?.Id;
    }

    public int GetHashCode(Song obj)
    {
        return obj.Id.GetHashCode();
    }
}