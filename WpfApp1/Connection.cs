using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class Connection
    {
        static public SqlConnection Connect = 
            new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename="+ Path.GetFullPath(@"..\..\Database1.mdf") + @";Integrated Security=True");
    }
}
