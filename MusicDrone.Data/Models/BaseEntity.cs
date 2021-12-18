using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicDrone.Data.Models
{
    public abstract class BaseEntity<T>
    {
        public virtual T Id { get; protected set; }
    }
}