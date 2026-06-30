// TaskItem.cs — represents one cybersecurity task

namespace MyBasicChatbot.Helpers
{
    public class TaskItem
    {
        public int Id { get; set; }

        // Short task name e.g. "Enable 2FA"
        public string Title { get; set; } = "";

        // Longer description of what needs to be done
        public string Description { get; set; } = "";

        // Whether the user has completed the task
        public bool IsCompleted { get; set; } = false;

        // Optional reminder date
        public DateTime? ReminderDate { get; set; }

        // When the task was created
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Used in the task list display
        public string StatusText => IsCompleted ? "✅ Done" : "⏳ Pending";

        public string ReminderText => ReminderDate.HasValue
            ? $"🔔 {ReminderDate.Value:dd MMM yyyy}"
            : "No reminder";
    }
}
