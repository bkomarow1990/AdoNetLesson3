﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetLesson3
{
    class ShopDb
    {
        static public SqlConnection connection;
        public SqlCommand command;
        public SqlDataAdapter da = null;
        // DataSet
        public DataSet set = null;
        public ShopDb()
        {
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ShopDB"].ConnectionString);
            command = new SqlCommand("select * from Users", connection); ;
        }
    }
}
