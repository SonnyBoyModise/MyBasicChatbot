// DatabaseHelper.cs — handles all MySQL database operations for tasks
// If MySQL is not available, tasks are stored in memory instead

using MySqlConnector;

namespace MyBasicChatbot.Helpers
{
    public class DatabaseHelper
    {
        // Change this connection string to match your MySQL setup
        // Server = localhost, Database = cyberbotdb, User = root, Password = your password
        private const string ConnectionString =
            "Server=localhost;Database=cyberbotdb;User ID=root;Password=;";

        // True if we successfully connected to MySQL
        public bool IsConnected { get; private set; } = false;

        // In-memory fallback list used when MySQL is not available
        private readonly List<TaskItem> _memoryTasks = new();
        private int _nextId = 1;

        // Constructor — tries to connect and create the table
        public DatabaseHelper()
        {
            TryConnect();
        }

        // Tries to connect to MySQL and create the tasks table
        private void TryConnect()
        {
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();

                // Create the database table if it doesn't exist yet
                string sql = @"
                    CREATE TABLE IF NOT EXISTS Tasks (
                        Id          INT AUTO_INCREMENT PRIMARY KEY,
                        Title       VARCHAR(255) NOT NULL,
                        Description TEXT,
                        IsCompleted TINYINT(1)   DEFAULT 0,
                        ReminderDate DATETIME    NULL,
                        CreatedAt   DATETIME     DEFAULT CURRENT_TIMESTAMP
                    );";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                IsConnected = true;
            }
            catch
            {
                // MySQL not available — will use in-memory storage instead
                IsConnected = false;
            }
        }

        // Adds a new task to the database or memory
        public void AddTask(TaskItem task)
        {
            if (IsConnected)
            {
                try
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();

                    string sql = "INSERT INTO Tasks (Title, Description, ReminderDate) VALUES (@t, @d, @r)";
                    using var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@t", task.Title);
                    cmd.Parameters.AddWithValue("@d", task.Description);
                    cmd.Parameters.AddWithValue("@r", (object?)task.ReminderDate ?? DBNull.Value);
                    cmd.ExecuteNonQuery();

                    // Get the auto-generated ID back
                    task.Id = (int)cmd.LastInsertedId;
                    return;
                }
                catch { /* fall through to memory storage */ }
            }

            // Memory fallback
            task.Id = _nextId++;
            _memoryTasks.Add(task);
        }

        // Returns all tasks
        public List<TaskItem> GetAllTasks()
        {
            if (IsConnected)
            {
                try
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();

                    string sql = "SELECT Id, Title, Description, IsCompleted, ReminderDate, CreatedAt FROM Tasks ORDER BY CreatedAt DESC";
                    using var cmd  = new MySqlCommand(sql, conn);
                    using var reader = cmd.ExecuteReader();

                    var tasks = new List<TaskItem>();
                    while (reader.Read())
                    {
                        tasks.Add(new TaskItem
                        {
                            Id          = reader.GetInt32("Id"),
                            Title       = reader.GetString("Title"),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString("Description"),
                            IsCompleted = reader.GetInt32("IsCompleted") == 1,
                            ReminderDate = reader.IsDBNull(reader.GetOrdinal("ReminderDate")) ? null : reader.GetDateTime("ReminderDate"),
                            CreatedAt   = reader.GetDateTime("CreatedAt")
                        });
                    }
                    return tasks;
                }
                catch { /* fall through */ }
            }

            // Return in-memory list
            return new List<TaskItem>(_memoryTasks);
        }

        // Marks a task as completed
        public void CompleteTask(int id)
        {
            if (IsConnected)
            {
                try
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    using var cmd = new MySqlCommand("UPDATE Tasks SET IsCompleted = 1 WHERE Id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    return;
                }
                catch { /* fall through */ }
            }

            // Memory fallback
            var task = _memoryTasks.FirstOrDefault(t => t.Id == id);
            if (task != null) task.IsCompleted = true;
        }

        // Deletes a task by its ID
        public void DeleteTask(int id)
        {
            if (IsConnected)
            {
                try
                {
                    using var conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    using var cmd = new MySqlCommand("DELETE FROM Tasks WHERE Id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    return;
                }
                catch { /* fall through */ }
            }

            // Memory fallback
            _memoryTasks.RemoveAll(t => t.Id == id);
        }
    }
}
