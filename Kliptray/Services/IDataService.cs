using Kliptray.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kliptray.Services;

public interface IDataService
{
    Task<List<Message>> GetChatsAsync(string id);
    Task AddNewMessage(string id, Message message);
}
