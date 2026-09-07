import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import taskService from "../../services/taskService";
import "./Tasks.css";

function TaskDetails() {
    const { projectId, taskId } = useParams();
    const { user, logout } = useAuth();
    const navigate = useNavigate();

    const [task, setTask] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        loadTask();
    }, [projectId, taskId]);

    const loadTask = async () => {
        try {
            setLoading(true);
            setError("");

            const data = await taskService.getById(projectId, taskId);
            setTask(data);
        } catch (err) {
            console.error(err);

            if (err.response?.status === 401) {
                logout();
                navigate("/login");
                return;
            }

            if (err.response?.status === 403) {
                setError("You do not have permission to view this task.");
                return;
            }

            if (err.response?.status === 404) {
                setError("Task not found.");
                return;
            }

            setError(
                err.response?.data?.message ||
                "Failed to load task."
            );
        } finally {
            setLoading(false);
        }
    };

    const handleLogout = () => {
        logout();
        navigate("/login");
    };

    const role = user?.role || "Member";

    const isAdmin = role.toLowerCase() === "admin";
    const isManager = role.toLowerCase() === "manager";

    const formatDate = (date) => {
        if (!date) {
            return "Not set";
        }

        return new Date(date).toLocaleDateString();
    };

    const getStatusClass = (status) => {
        switch (status?.toLowerCase()) {
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

    const getPriorityClass = (priority) => {
        switch (priority?.toLowerCase()) {
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

    if (loading) {
        return (
            <div className="dashboard-page">
                <aside className="dashboard-sidebar">
                    <div className="sidebar-brand">
                        <h1>
                            Task<span>Pulse</span>
                        </h1>
                    </div>
                </aside>

                <main className="dashboard-main">
                    <div className="task-details-loading">
                        Loading task...
                    </div>
                </main>
            </div>
        );
    }

    return (
        <div className="dashboard-page">

            {/* Sidebar */}
            <aside className="dashboard-sidebar">

                <div className="sidebar-brand">
                    <h1>
                        Task<span>Pulse</span>
                    </h1>
                </div>

                <nav className="sidebar-nav">

                    <button
                        className="nav-item"
                        onClick={() => navigate("/dashboard")}
                    >
                        <span className="nav-icon">⌂</span>
                        Dashboard
                    </button>

                    <button
                        className="nav-item"
                        onClick={() => navigate("/projects")}
                    >
                        <span className="nav-icon">▣</span>
                        Projects
                    </button>

                    <button
                        className="nav-item active"
                        onClick={() =>
                            navigate(
                                `/projects/${projectId}/tasks`
                            )
                        }
                    >
                        <span className="nav-icon">✓</span>
                        Tasks
                    </button>

                    <button
                        className="nav-item"
                        onClick={() => navigate("/notifications")}
                    >
                        <span className="nav-icon">●</span>
                        Notifications
                    </button>

                    {(isAdmin || isManager) && (
                        <button
                            className="nav-item"
                            onClick={() => navigate("/reports")}
                        >
                            <span className="nav-icon">▤</span>
                            Reports
                        </button>
                    )}

                    {isAdmin && (
                        <button
                            className="nav-item"
                            onClick={() => navigate("/users")}
                        >
                            <span className="nav-icon">♙</span>
                            Users
                        </button>
                    )}

                    {isAdmin && (
                        <button
                            className="nav-item"
                            onClick={() => navigate("/activity-logs")}
                        >
                            <span className="nav-icon">☷</span>
                            Activity Logs
                        </button>
                    )}

                </nav>

                <div className="sidebar-bottom">

                    <button
                        className="nav-item"
                        onClick={handleLogout}
                    >
                        <span className="nav-icon">↪</span>
                        Logout
                    </button>

                </div>

            </aside>

            {/* Main */}
            <main className="dashboard-main">

                {/* Header */}
                <header className="dashboard-header">

                    <div>
                        <h2>Task Details</h2>

                        <p>
                            View task information and activity
                        </p>
                    </div>

                    <div className="user-info">

                        <div className="user-avatar">
                            {(user?.fullName || user?.email || "U")
                                .charAt(0)
                                .toUpperCase()}
                        </div>

                        <div className="user-details">

                            <strong>
                                {user?.fullName || "User"}
                            </strong>

                            <span>
                                {role}
                            </span>

                        </div>

                    </div>

                </header>

                {/* Content */}
                <section className="task-details-content">

                    {/* Back */}
                    <button
                        className="task-details-back"
                        onClick={() =>
                            navigate(
                                `/projects/${projectId}/tasks`
                            )
                        }
                    >
                        ← Back to Tasks
                    </button>

                    {error ? (
                        <div className="task-details-error">
                            {error}
                        </div>
                    ) : task ? (
                        <>
                            {/* Task Heading */}
                            <div className="task-details-title-row">

                                <div>
                                    <h1>
                                        {task.title}
                                    </h1>

                                    <p>
                                        Task details
                                    </p>
                                </div>

                                {(isAdmin || isManager) && (
                                    <button
                                        className="task-details-primary-button"
                                        onClick={() =>
                                            navigate(
                                                `/projects/${projectId}/tasks/${taskId}/edit`
                                            )
                                        }
                                    >
                                        Edit Task
                                    </button>
                                )}

                            </div>

                            {/* Information Card */}
                            <div className="task-details-card">

                                <div className="task-details-card-header">
                                    <div>
                                        <h3>Task Information</h3>
                                        <p>
                                            Details about this task
                                        </p>
                                    </div>
                                </div>

                                <div className="task-details-info-grid">

                                    <div className="task-details-info-item">
                                        <span>Title</span>

                                        <strong>
                                            {task.title}
                                        </strong>
                                    </div>

                                    <div className="task-details-info-item">
                                        <span>Status</span>

                                        <div>
                                            <span
                                                className={getStatusClass(
                                                    task.status
                                                )}
                                            >
                                                {task.status}
                                            </span>
                                        </div>
                                    </div>

                                    <div className="task-details-info-item">
                                        <span>Priority</span>

                                        <div>
                                            <span
                                                className={getPriorityClass(
                                                    task.priority
                                                )}
                                            >
                                                {task.priority}
                                            </span>
                                        </div>
                                    </div>

                                    <div className="task-details-info-item">
                                        <span>Assigned To</span>

                                        <strong>
                                            {task.assignedToUserName ||
                                                "Unassigned"}
                                        </strong>
                                    </div>

                                    <div className="task-details-info-item">
                                        <span>Created By</span>

                                        <strong>
                                            {task.createdByUserName ||
                                                "Unknown"}
                                        </strong>
                                    </div>

                                    <div className="task-details-info-item">
                                        <span>Due Date</span>

                                        <strong>
                                            {formatDate(task.dueDate)}
                                        </strong>
                                    </div>

                                    <div className="task-details-info-item">
                                        <span>Created At</span>

                                        <strong>
                                            {formatDate(task.createdAt)}
                                        </strong>
                                    </div>

                                    <div className="task-details-info-item">
                                        <span>Updated At</span>

                                        <strong>
                                            {formatDate(task.updatedAt)}
                                        </strong>
                                    </div>

                                </div>

                            </div>

                            {/* Description */}
                            <div className="task-details-card">

                                <div className="task-details-card-header">
                                    <div>
                                        <h3>Description</h3>

                                        <p>
                                            Task description
                                        </p>
                                    </div>
                                </div>

                                <div className="task-details-description">
                                    {task.description ||
                                        "No description provided."}
                                </div>

                            </div>
                        </>
                    ) : (
                        <div className="task-details-error">
                            Task not found.
                        </div>
                    )}

                </section>

            </main>

        </div>
    );
}

export default TaskDetails;