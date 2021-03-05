
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using Orca.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orca.Entities;



namespace Orca.Database
{
    public class DatabaseConnect : IDisposable
    {
        private string _servername;
        private string _uid;
        private string _password;
        private string _database;
        private bool _hasDatabase = false;
        public bool _disposedValue;
        public MySqlConnection Connection { get; set; }
   
        public DatabaseConnect(IOptions<DatabaseFields> settings)
        {
            var fields = settings.Value;
            _servername = fields.Servername;
            _uid = fields.Uid;
            _password = fields.Password;
            _database = fields.Database;
            string connString = $"Server = {_servername}; Database = {_database}; Uid= {_uid}; Pwd= {_password};";
            Connection = new MySqlConnection(connString);
            try
            {
                Connection.Open();
                _hasDatabase = true;
            
            }
            catch (MySqlException ex)
            {
                if (_servername != null && _uid != null && _password != null && _database !=null)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public bool HasDatabase()
        {
            return _hasDatabase;
        }
     

    
   

     
        public async Task StoreEventToDatabase(StudentEvent studentEvent)
        {
            string sql = "INSERT INTO event(student_ID, course_ID, Timestamp, event_type, activity_name, activity_details) VALUES(@student_ID,@course_ID, @Timestamp, @event_type, @activity_name, @activity_details)";
            MySqlParameter[] parameters = {
                                            new MySqlParameter("@student_ID", studentEvent.Student.ID),
                                            new MySqlParameter("@course_ID", studentEvent.CourseID),
                                            new MySqlParameter("@Timestamp", studentEvent.Timestamp),
                                            new MySqlParameter("@event_type", studentEvent.EventType),
                                            new MySqlParameter("@activity_name", studentEvent.ActivityName),
                                            new MySqlParameter("@activity_details", studentEvent.ActivityType)
                                        };

         
 
            await AddInfoToDatabase(sql, parameters);
            
            
        }

        public async Task StoreStudentToDatabase(StudentEvent studentEvent)
        {
            
            string sql = "INSERT IGNORE INTO student(id, first_name, last_name,email) VALUES(@ID, @first_name, @last_name,@studentEmail)";
            MySqlParameter[] parameters = {
                                                        new MySqlParameter("@ID", studentEvent.Student.ID),
                                                        new MySqlParameter("@first_name", studentEvent.Student.FirstName),
                                                        new MySqlParameter("@last_name", studentEvent.Student.LastName),
                                                        new MySqlParameter("@studentEmail", studentEvent.Student.Email)
                                                    };
    
            
          
            await AddInfoToDatabase(sql, parameters);
         
        }

        public async Task AddInfoToDatabase(string sql, MySqlParameter[] parameters)
        {
            
                using (MySqlCommand cmd = new MySqlCommand(sql, Connection))
                {
                try
                {
                    cmd.Parameters.AddRange(parameters);
                    cmd.ExecuteScalar();
                    Console.WriteLine("Transaction successful");
                    
                }
                catch(Exception e)
                {

                    if (_servername != null && _uid != null && _password != null && _database != null)
                    {
                        Console.WriteLine(e.Message);
                    }

                }
            }
           
        }
      
        protected virtual void Dispose(bool disposing)
        {
           
            if (!_disposedValue)
            {
                if (disposing)
                {
                  
                   Connection?.Dispose();

              
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }
    }
}
