namespace api_jogos.Model.Entities
{
    public class BaseEntity
    {
        public BaseEntity() { }
        public BaseEntity(Guid id) => Id = id;

        public Guid Id { get; private set; }            
    }
}
