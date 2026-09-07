import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import taskService from "../../services/taskService";
import projectMemberService from "../../services/projectMemberService";
import "./Tasks.css";

function CreateTask() {
    const { projectId } = useParams();
    const { user, logout } = useAuth();
    const navigate = useNavigate();

    const [members, setMembers] = useState([]);

    const [formData, setFormData] = useState({
        title: "",
        description: "",
        status: "Todo",
        priority: "Medium",
        assignedToUserId: "",
        dueDate: "",
    });

    const [loading, setLoading] = useState(false);
    const [membersLoading, setMembersLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        loadMembers();
    }, [projectId]);

    const loadMembers = async () => {
        try {
            setMembersLoading(true);
            setError("");

            const data = await projectMemberService.getByProject(projectId);

            setMembers(data || []);
        } catch (err) {
            console.error(err);

            if (err.response?.status === 401) {
                logout();
                navigate("/login");
                return;
            }

            if (err.response?.status === 403) {
                setError("You do not have permission to view project members.");
                return;
            }

            setError(
                err.response?.data?.message ||
                "Failed to load project members."
            );
        } finally {
            setMembersLoading(false);
        }
    };

    const handleChange = (e) => {
        const { name, value } = e.target;

        setFormData((previous) => ({
            ...previous,
            [name]: value,
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!formData.title.trim()) {
            setError("Task title is required.");
            return;
        }

        try {
            setLoading(true);
            setError("");

            await taskService.create(projectId, {
                title: formData.title.trim(),
                description: formData.description.trim() || null,
                status: formData.status,
                priority: formData.priority,
                assignedToUserId:
                    formData.assignedToUserId || null,
                dueDate: formData.dueDate
                    ? new Date(formData.dueDate).toISOString()
                    : null,
            });

            navigate(`/projects/${projectId}/tasks`);
        } catch (err) {
            console.error(err);

            if (err.response?.status === 401) {
                logout();
                navigate("/login");
                return;
            }

            if (err.response?.status === 403) {
                setError(
                    "You do not have permission to create tasks in this project."
                );
                return;
            }

            if (err.response?.status === 400) {
                setError(
                    err.response?.data?.message ||
                    "Please check the task information."
                );
                return;
            }

            setError(
                err.response?.data?.message ||
                "Failed to create task."
            );
        } finally {
            setLoading(false);
        }
    };

    const handleCancel = () => {
        navigate(`/projects/${projectId}/tasks`);
    };

    const handleLogout = () => {
        logout();
        navigate("/login");
    };

    const role = user?.role || "Member";

    const isAdmin =
        role.toLowerCase() === "admin";

    const isManager =
        role.toLowerCase() === "manager";

    return (
        <div className="dashboard-page">

            {/* SIDEBAR */}
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
                            navigate(`/projects/${projectId}/tasks`)
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

            {/* MAIN */}
            <main className="dashboard-main">

                {/* HEADER */}
                <header className="dashboard-header">

                    <div>
                        <h2>Create Task</h2>
                        <p>
                            Create a new task for this project
                        </p>
                    </div>

                    <div className="user-info">

                        <div className="user-avatar">
                            {(user?.fullName ||
                                user?.email ||
                                "U")
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

                {/* CONTENT */}
                <section className="create-task-content">

                    <button
                        className="create-task-back"
                        onClick={handleCancel}
                    >
                        ← Back to Tasks
                    </button>

                    <div className="create-task-title">

                        <div>
                            <h1>Create Task</h1>

                            <p>
                                Add a new task to your project.
                            </p>
                        </div>

                    </div>

                    {error && (
                        <div className="create-task-error">
                            {error}
                        </div>
                    )}

                    <div className="create-task-card">

                        <div className="create-task-card-header">

                            <div>
                                <h3>Task Information</h3>

                                <p>
                                    Enter the details for the new task.
                                </p>
                            </div>

                        </div>

                        <form
                            className="create-task-form"
                            onSubmit={handleSubmit}
                        >

                            {/* TITLE */}
                            <div className="create-task-field full-width">

                                <label htmlFor="title">
                                    Task Title
                                </label>

                                <input
                                    id="title"
                                    name="title"
                                    type="text"
                                    value={formData.title}
                                    onChange={handleChange}
                                    placeholder="Enter task title"
                                    disabled={loading}
                                    required
                                />

                            </div>

                            {/* DESCRIPTION */}
                            <div className="create-task-field full-width">

                                <label htmlFor="description">
                                    Description
                                </label>

                                <textarea
                                    id="description"
                                    name="description"
                                    value={formData.description}
                                    onChange={handleChange}
                                    placeholder="Enter task description"
                                    rows="5"
                                    disabled={loading}
                                />

                            </div>

                            {/* STATUS */}
                            <div className="create-task-field">

                                <label htmlFor="status">
                                    Status
                                </label>

                                <select
                                    id="status"
                                    name="status"
                                    value={formData.status}
                                    onChange={handleChange}
                                    disabled={loading}
                                >
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

                            </div>

                            {/* PRIORITY */}
                            <div className="create-task-field">

                                <label htmlFor="priority">
                                    Priority
                                </label>

                                <select
                                    id="priority"
                                    name="priority"
                                    value={formData.priority}
                                    onChange={handleChange}
                                    disabled={loading}
                                >
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

                            </div>

                            {/* ASSIGNED USER */}
                            <div className="create-task-field">

                                <label htmlFor="assignedToUserId">
                                    Assign To
                                </label>

                                <select
                                    id="assignedToUserId"
                                    name="assignedToUserId"
                                    value={formData.assignedToUserId}
                                    onChange={handleChange}
                                    disabled={
                                        loading ||
                                        membersLoading
                                    }
                                >

                                    <option value="">
                                        Unassigned
                                    </option>

                                    {members.map((member) => (
                                        <option
                                            key={
                                                member.userId ||
                                                member.id
                                            }
                                            value={
                                                member.userId
                                            }
                                        >
                                            {member.userName ||
                                                member.fullName ||
                                                member.email ||
                                                "Project Member"}
                                        </option>
                                    ))}

                                </select>

                            </div>

                            {/* DUE DATE */}
                            <div className="create-task-field">

                                <label htmlFor="dueDate">
                                    Due Date
                                </label>

                                <input
                                    id="dueDate"
                                    name="dueDate"
                                    type="date"
                                    value={formData.dueDate}
                                    onChange={handleChange}
                                    disabled={loading}
                                />

                            </div>

                            {/* BUTTONS */}
                            <div className="create-task-actions">

                                <button
                                    type="button"
                                    className="create-task-cancel"
                                    onClick={handleCancel}
                                    disabled={loading}
                                >
                                    Cancel
                                </button>

                                <button
                                    type="submit"
                                    className="create-task-submit"
                                    disabled={loading}
                                >
                                    {loading
                                        ? "Creating..."
                                        : "Create Task"}
                                </button>

                            </div>

                        </form>

                    </div>

                </section>

            </main>

        </div>
    );
}

export default CreateTask;