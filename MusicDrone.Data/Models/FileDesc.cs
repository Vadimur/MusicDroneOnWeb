using System;

namespace MusicDrone.Data.Models
{
    public class FileDesc : BaseEntity<Guid>
    {
        public string FilePath { get; set; }
        public bool IsRemoved { get; set; }
    }
}