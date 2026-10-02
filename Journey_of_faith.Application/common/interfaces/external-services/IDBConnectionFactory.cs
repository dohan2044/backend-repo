using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Journey_of_faith.Application.common.interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
