using Kliptray.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kliptray.Services;

/// <summary>
/// Use LiteDB for data persistance
/// </summary>
public class DataService : IDataService
{
    public Task AddNewMessage(string id, Message message)
    {
        throw new NotImplementedException();
    }

    public Task<List<Message>> GetChatsAsync(string id)
    {
        throw new NotImplementedException();
    }
}
