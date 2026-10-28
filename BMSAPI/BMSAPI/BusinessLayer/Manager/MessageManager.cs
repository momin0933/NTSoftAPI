using BMSAPI.BusinessLayer.Interface;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models;

namespace BMSAPI.BusinessLayer.Manager
{
    public class MessageManager : IMessage
    {
        private readonly IDapperService _dapper;

        public MessageManager(IDapperService dapper)
        {
            _dapper = dapper;
        }

        public int Add(SendMessage entity)
        {
            if (string.IsNullOrWhiteSpace(entity.PhoneNumber))
                throw new ArgumentException("Phone number is required.");

            return _dapper.GetSingle<int>(@"
                INSERT INTO dbo.tblSmsQueue (AppName, PhoneNumber, Message, Status, CreatedAt)
                VALUES (@AppName, @PhoneNumber, @Message, @Status, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS int);",
                new
                {
                    entity.AppName,
                    entity.PhoneNumber,
                    entity.Message,
                    entity.Status,
                    CreatedAt = DateTime.Now
                });
        }

        public List<SendMessage> GetAll() => throw new NotImplementedException();

        public SendMessage? Get(int id) => throw new NotImplementedException();

        public int Update(SendMessage entity) => throw new NotImplementedException();

        public int Remove(SendMessage entity) => throw new NotImplementedException();
    }
}