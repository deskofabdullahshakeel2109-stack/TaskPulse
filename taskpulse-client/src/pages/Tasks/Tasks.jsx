import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import taskService from "../../services/taskService";
import "./Tasks.css";

function Tasks() {
    const { user, logout } = useAuth();
    const { projectId } = useParams();
    const navigate = useNavigate();

    const [tasks, setTasks] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [search, setSearch] = useState("");
    const [status, setStatus] = useState("");
    const [priority, setPriority] = useState("");
    const [sortBy, setSortBy] = useState("CreatedAt");
    const [sortOrder, setSortOrder] = useState("desc");

    const role = user?.role || "Member";

    const canManageTasks =
        role === "Admin" ||
        role === "Manager";

    const loadTasks = async () => {
        if (!projectId) {
            setError("No project was selected.");
            setLoading(false);
            return;
        }

        try {
            setLoading(true);
            setError("");

            const data = await taskService.getByProject(projectId, {
                Search: search || undefined,
                Status: status || undefined,
                Priority: priority || undefined,
                SortBy: sortBy,
                SortOrder: sortOrder,
                Page: 1,
                PageSize: 100,
            });

            setTasks(Array.isArray(data) ? data : []);
        } catch (err) {
            console.error("Tasks error:", err);

            setError(
                err.response?.data?.message ||
                "Unable to load tasks."
            );
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadTasks();
    }, [projectId, status, priority, sortBy, sortOrder]);

    const handleLogout = () => {
        logout();
        navigate("/login");
    };

    const handleSearch = (event) => {
        if (event.key === "Enter") {
            loadTasks();
        }
    };

    const handleDelete = async (taskId) => {
        const confirmed = window.confirm(
            "Are you sure you want to delete this task?"
        );

        if (!confirmed) {
            return;
        }

        try {
            await taskService.delete(projectId, taskId);
            await loadTasks();
        } catch (err) {
            console.error("Delete task error:", err);

            alert(
                err.response?.data?.message ||
                "Unable to delete task."
            );
        }
    };

    const getStatusClass = (taskStatus) => {
        switch (taskStatus?.toLowerCase()) {
            case "completed":
                return "task-status completed";

            case "inprogress":
                return "task-status in-progress";

            case "todo":
                return "task-status todo";

            default:
                return "task-status";
        }
    };

    const getPriorityClass = (taskPriority) => {
        switch (taskPriority?.toLowerCase()) {
            case "high":
                return "task-priority high";

            case "medium":
                return "task-priority medium";

            case "low":
                return "task-priority low";

            default:
                return "task-priority";
        }
    };

    const formatDate = (date) => {
        if (!date) {
            return "-";
        }

        return new Date(date).toLocaleDateString();
    };

    const formatStatus = (taskStatus) => {
        if (!taskStatus) {
            return "-";
        }

        if (taskStatus === "InProgress") {
            return "In Progress";
        }

        if (taskStatus === "Todo") {
            return "To Do";
        }

        return taskStatus;
    };

    return (
        <div className="tasks-page">

            {/* SIDEBAR */}
            <aside className="tasks-sidebar">

                <div className="tasks-brand">
                    <h1>
                        Task<span>Pulse</span>
                    </h1>
                </div>

                <nav className="tasks-nav">

                    <button
                        className="tasks-nav-item"
                        onClick={() => navigate("/dashboard")}
                    >
                        <span>⌂</span>
                        Dashboard
                    </button>

                    <button
                        className="tasks-nav-item"
                        onClick={() => navigate("/projects")}
                    >
                        <span>▣</span>
                        Projects
                    </button>

                    <button
                        className="tasks-nav-item active"
                    >
                        <span>✓</span>
                        Tasks
                    </button>

                    <button
                        className="tasks-nav-item"
                        onClick={() => navigate("/notifications")}
                    >
                        <span>●</span>
                        Notifications
                    </button>

                    {(role === "Admin" || role === "Manager") && (
                        <button
                            className="tasks-nav-item"
                            onClick={() => navigate("/reports")}
                        >
                            <span>▤</span>
                            Reports
                        </button>
                    )}

                    {role === "Admin" && (
                        <>
                            <button
                                className="tasks-nav-item"
                                onClick={() => navigate("/users")}
                            >
                                <span>♙</span>
                                Users
                            </button>

                            <button
                                className="tasks-nav-item"
                                onClick={() => navigate("/activity-logs")}
                            >
                                <span>☷</span>
                                Activity Logs
                            </button>
                        </>
                    )}

                </nav>

                <div className="tasks-sidebar-bottom">
                    <button
                        className="tasks-nav-item"
                        onClick={handleLogout}
                    >
                        <span>↪</span>
                        Logout
                    </button>
                </div>

            </aside>

            {/* MAIN */}
            <main className="tasks-main">

                <header className="tasks-header">

                    <div>
                        <button
                            className="tasks-back-button"
                            onClick={() => navigate("/projects")}
                        >
                            ← Back to Projects
                        </button>

                        <h2>Tasks</h2>

                        <p>
                            Manage tasks for this project
                        </p>
                    </div>

                    <div className="tasks-header-right">

                        <div className="tasks-user">

                            <div className="tasks-avatar">
                                {(
                                    user?.fullName ||
                                    user?.email ||
                                    "U"
                                )
                                    .charAt(0)
                                    .toUpperCase()}
                            </div>

                            <div className="tasks-user-text">
                                <strong>
                                    {user?.fullName || "User"}
                                </strong>

                                <span>
                                    {role}
                                </span>
                            </div>

                        </div>

                        {canManageTasks && (
                            <button
                                className="tasks-create-button"
                                onClick={() =>
                                    navigate(
                                        `/projects/${projectId}/tasks/create`
                                    )
                                }
                            >
                                + Create Task
                            </button>
                        )}

                    </div>

                </header>

                <div className="tasks-content">

                    {/* FILTERS */}
                    <section className="tasks-filter-card">

                        <div className="tasks-search">

                            <input
                                type="text"
                                placeholder="Search tasks..."
                                value={search}
                                onChange={(event) =>
                                    setSearch(event.target.value)
                                }
                                onKeyDown={handleSearch}
                            />

                            <button
                                onClick={loadTasks}
                                className="tasks-search-button"
                            >
                                Search
                            </button>

                        </div>

                        <div className="tasks-filter-row">

                            <select
                                value={status}
                                onChange={(event) =>
                                    setStatus(event.target.value)
                                }
                            >
                                <option value="">
                                    All Statuses
                                </option>

                                <option value="Todo">
                                    To Do
                                </option>

                                <option value="InProgress">
                                    In Progress
                                </option>

                                <option value="Completed">
                                    Completed
                                </option>
                            </select>

                            <select
                                value={priority}
                                onChange={(event) =>
                                    setPriority(event.target.value)
                                }
                            >
                                <option value="">
                                    All Priorities
                                </option>

                                <option value="Low">
                                    Low
                                </option>

                                <option value="Medium">
                                    Medium
                                </option>

                                <option value="High">
                                    High
                                </option>
                            </select>

                            <select
                                value={sortBy}
                                onChange={(event) =>
                                    setSortBy(event.target.value)
                                }
                            >
                                <option value="CreatedAt">
                                    Created Date
                                </option>

                                <option value="Title">
                                    Title
                                </option>

                                <option value="Status">
                                    Status
                                </option>

                                <option value="Priority">
                                    Priority
                                </option>

                                <option value="DueDate">
                                    Due Date
                                </option>

                                <option value="UpdatedAt">
                                    Updated Date
                                </option>
                            </select>

                            <select
                                value={sortOrder}
                                onChange={(event) =>
                                    setSortOrder(event.target.value)
                                }
                            >
                                <option value="desc">
                                    Newest First
                                </option>

                                <option value="asc">
                                    Oldest First
                                </option>
                            </select>

                        </div>

                    </section>

                    {/* ERROR */}
                    {error && (
                        <div className="tasks-error">
                            <h3>
                                Unable to load tasks
                            </h3>

                            <p>{error}</p>

                            <button
                                onClick={loadTasks}
                                className="tasks-retry-button"
                            >
                                Try Again
                            </button>
                        </div>
                    )}

                    {/* LOADING */}
                    {loading && !error && (
                        <div className="tasks-loading">
                            <div className="tasks-spinner"></div>

                            <p>
                                Loading tasks...
                            </p>
                        </div>
                    )}

                    {/* TASKS */}
                    {!loading && !error && (
                        <section className="tasks-list-card">

                            <div className="tasks-list-header">

                                <div>
                                    <h3>
                                        Project Tasks
                                    </h3>

                                    <p>
                                        {tasks.length} task
                                        {tasks.length !== 1
                                            ? "s"
                                            : ""}
                                    </p>
                                </div>

                                <button
                                    className="tasks-refresh-button"
                                    onClick={loadTasks}
                                >
                                    ↻ Refresh
                                </button>

                            </div>

                            {tasks.length === 0 ? (
                                <div className="tasks-empty">

                                    <div className="tasks-empty-icon">
                                        ✓
                                    </div>

                                    <h3>
                                        No tasks found
                                    </h3>

                                    <p>
                                        There are currently no tasks
                                        matching your filters.
                                    </p>

                                    {canManageTasks && (
                                        <button
                                            className="tasks-create-empty-button"
                                            onClick={() =>
                                                navigate(
                                                    `/projects/${projectId}/tasks/create`
                                                )
                                            }
                                        >
                                            + Create First Task
                                        </button>
                                    )}

                                </div>
                            ) : (
                                <div className="tasks-table-wrapper">

                                    <table className="tasks-table">

                                        <thead>
                                            <tr>
                                                <th>Task</th>
                                                <th>Status</th>
                                                <th>Priority</th>
                                                <th>Assigned To</th>
                                                <th>Due Date</th>
                                                <th>Created By</th>
                                                <th>Actions</th>
                                            </tr>
                                        </thead>

                                        <tbody>

                                            {tasks.map((task) => (
                                                <tr key={task.id}>

                                                    <td>
                                                        <div className="task-title-cell">

                                                            <strong>
                                                                {task.title}
                                                            </strong>

                                                            {task.description && (
                                                                <span>
                                                                    {task.description.length > 80
                                                                        ? `${task.description.substring(
                                                                            0,
                                                                            80
                                                                        )}...`
                                                                        : task.description}
                                                                </span>
                                                            )}

                                                        </div>
                                                    </td>

                                                    <td>
                                                        <span
                                                            className={getStatusClass(
                                                                task.status
                                                            )}
                                                        >
                                                            {formatStatus(
                                                                task.status
                                                            )}
                                                        </span>
                                                    </td>

                                                    <td>
                                                        <span
                                                            className={getPriorityClass(
                                                                task.priority
                                                            )}
                                                        >
                                                            {task.priority}
                                                        </span>
                                                    </td>

                                                    <td>
                                                        {task.assignedToUserName ||
                                                            "Unassigned"}
                                                    </td>

                                                    <td>
                                                        {formatDate(
                                                            task.dueDate
                                                        )}
                                                    </td>

                                                    <td>
                                                        {task.createdByUserName ||
                                                            "-"}
                                                    </td>

                                                    <td>
                                                        <div className="task-actions">

                                                            <button
                                                                className="task-action-view"
                                                                onClick={() =>
                                                                    navigate(
                                                                        `/projects/${projectId}/tasks/${task.id}`
                                                                    )
                                                                }
                                                            >
                                                                View
                                                            </button>

                                                            {canManageTasks && (
                                                                <>
                                                                    <button
                                                                        className="task-action-edit"
                                                                        onClick={() =>
                                                                            navigate(
                                                                                `/projects/${projectId}/tasks/${task.id}/edit`
                                                                            )
                                                                        }
                                                                    >
                                                                        Edit
                                                                    </button>

                                                                    <button
                                                                        className="task-action-delete"
                                                                        onClick={() =>
                                                                            handleDelete(
                                                                                task.id
                                                                            )
                                                                        }
                                                                    >
                                                                        Delete
                                                                    </button>
                                                                </>
                                                            )}

                                                        </div>
                                                    </td>

                                                </tr>
                                            ))}

                                        </tbody>

                                    </table>

                                </div>
                            )}

                        </section>
                    )}

                </div>

            </main>

        </div>
    );
}

export default Tasks;