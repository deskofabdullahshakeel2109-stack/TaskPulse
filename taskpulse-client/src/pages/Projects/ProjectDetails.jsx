import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import projectService from "../../services/projectService";
import dashboardService from "../../services/dashboardService";
import "./ProjectDetails.css";

function ProjectDetails() {
    const { user, logout } = useAuth();
    const { projectId } = useParams();
    const navigate = useNavigate();

    const [project, setProject] = useState(null);
    const [dashboard, setDashboard] = useState(null);

    const [loading, setLoading] = useState(true);
    const [dashboardLoading, setDashboardLoading] = useState(true);
    const [error, setError] = useState("");
    const [dashboardError, setDashboardError] = useState("");

    const role = user?.role || "Member";

    useEffect(() => {
        loadProject();
        loadDashboard();
    }, [projectId]);

    const loadProject = async () => {
        try {
            setLoading(true);
            setError("");

            const data = await projectService.getById(projectId);

            setProject(data);
        } catch (err) {
            setError(
                err.response?.data?.message ||
                "Unable to load project."
            );
        } finally {
            setLoading(false);
        }
    };

    const loadDashboard = async () => {
        try {
            setDashboardLoading(true);
            setDashboardError("");

            const data =
                await dashboardService.getProjectDashboard(projectId);

            setDashboard(data);
        } catch (err) {
            console.error("Dashboard error:", err);

            setDashboardError(
                err.response?.data?.message ||
                "Unable to load project statistics."
            );
        } finally {
            setDashboardLoading(false);
        }
    };

    const handleLogout = () => {
        logout();
        navigate("/login");
    };

    const totalTasks =
        dashboard?.totalTasks ??
        dashboard?.TotalTasks ??
        0;

    const completedTasks =
        dashboard?.completedTasks ??
        dashboard?.CompletedTasks ??
        0;

    const pendingTasks =
        dashboard?.pendingTasks ??
        dashboard?.PendingTasks ??
        0;

    const overdueTasks =
        dashboard?.overdueTasks ??
        dashboard?.OverdueTasks ??
        0;

    return (
        <div className="project-details-page">

            {/* Sidebar */}
            <aside className="project-details-sidebar">

                <div className="project-details-brand">
                    <h1>
                        Task<span>Pulse</span>
                    </h1>
                </div>

                <nav className="project-details-nav">

                    <button
                        className="project-details-nav-item"
                        onClick={() => navigate("/dashboard")}
                    >
                        <span>⌂</span>
                        Dashboard
                    </button>

                    <button
                        className="project-details-nav-item active"
                        onClick={() => navigate("/projects")}
                    >
                        <span>▣</span>
                        Projects
                    </button>

                    <button
                        className="project-details-nav-item"
                        onClick={() => navigate("/tasks")}
                    >
                        <span>✓</span>
                        Tasks
                    </button>

                    <button
                        className="project-details-nav-item"
                        onClick={() => navigate("/notifications")}
                    >
                        <span>●</span>
                        Notifications
                    </button>

                    {(role === "Admin" || role === "Manager") && (
                        <button
                            className="project-details-nav-item"
                            onClick={() => navigate("/reports")}
                        >
                            <span>▤</span>
                            Reports
                        </button>
                    )}

                    {role === "Admin" && (
                        <>
                            <button
                                className="project-details-nav-item"
                                onClick={() => navigate("/users")}
                            >
                                <span>♙</span>
                                Users
                            </button>

                            <button
                                className="project-details-nav-item"
                                onClick={() => navigate("/activity-logs")}
                            >
                                <span>☷</span>
                                Activity Logs
                            </button>
                        </>
                    )}

                </nav>

                <div className="project-details-sidebar-bottom">

                    <button
                        className="project-details-nav-item"
                        onClick={handleLogout}
                    >
                        <span>↪</span>
                        Logout
                    </button>

                </div>

            </aside>

            {/* Main */}
            <main className="project-details-main">

                {/* Header */}
                <header className="project-details-header">

                    <div>
                        <button
                            className="back-button"
                            onClick={() => navigate("/projects")}
                        >
                            ← Back to Projects
                        </button>

                        <h2>
                            {loading
                                ? "Project"
                                : project?.name || "Project"}
                        </h2>

                        <p>
                            View project information and activity
                        </p>
                    </div>

                    <div className="project-details-user">

                        <div className="project-details-avatar">
                            {(user?.fullName || user?.email || "U")
                                .charAt(0)
                                .toUpperCase()}
                        </div>

                        <div className="project-details-user-text">
                            <strong>
                                {user?.fullName || "User"}
                            </strong>

                            <span>{role}</span>
                        </div>

                    </div>

                </header>

                <div className="project-details-content">

                    {/* Loading */}
                    {loading && (
                        <div className="project-details-message">
                            <div className="project-details-spinner"></div>
                            <p>Loading project...</p>
                        </div>
                    )}

                    {/* Error */}
                    {!loading && error && (
                        <div className="project-details-error">

                            <h3>Unable to load project</h3>

                            <p>{error}</p>

                            <div className="project-details-error-actions">

                                <button
                                    onClick={loadProject}
                                    className="primary-button"
                                >
                                    Try Again
                                </button>

                                <button
                                    onClick={() => navigate("/projects")}
                                    className="secondary-button"
                                >
                                    Back to Projects
                                </button>

                            </div>

                        </div>
                    )}

                    {/* Project */}
                    {!loading && !error && project && (
                        <>
                            {/* Project Information */}
                            <section className="project-info-card">

                                <div className="project-info-top">

                                    <div className="project-info-icon">
                                        ▣
                                    </div>

                                    <div className="project-info-title">

                                        <div className="project-name-row">

                                            <h3>
                                                {project.name}
                                            </h3>

                                            <span className="project-active-badge">
                                                Active
                                            </span>

                                        </div>

                                        <p>
                                            {project.description ||
                                                "No description provided."}
                                        </p>

                                    </div>

                                </div>

                                <div className="project-info-details">

                                    <div className="project-info-detail">

                                        <span>
                                            Project ID
                                        </span>

                                        <strong>
                                            {project.id}
                                        </strong>

                                    </div>

                                    <div className="project-info-detail">

                                        <span>
                                            Created
                                        </span>

                                        <strong>
                                            {project.createdAt
                                                ? new Date(
                                                    project.createdAt
                                                ).toLocaleDateString()
                                                : "-"}
                                        </strong>

                                    </div>

                                    <div className="project-info-detail">

                                        <span>
                                            Created By
                                        </span>

                                        <strong>
                                            {project.createdByUserId ||
                                                "-"}
                                        </strong>

                                    </div>

                                </div>

                            </section>

                            {/* Statistics */}
                            <section className="project-stat-grid">

                                <div className="project-stat-card">

                                    <div className="project-stat-icon">
                                        ✓
                                    </div>

                                    <div>

                                        <span>
                                            Total Tasks
                                        </span>

                                        <strong>
                                            {dashboardLoading
                                                ? "..."
                                                : totalTasks}
                                        </strong>

                                    </div>

                                </div>

                                <div className="project-stat-card">

                                    <div className="project-stat-icon">
                                        ◷
                                    </div>

                                    <div>

                                        <span>
                                            Pending
                                        </span>

                                        <strong>
                                            {dashboardLoading
                                                ? "..."
                                                : pendingTasks}
                                        </strong>

                                    </div>

                                </div>

                                <div className="project-stat-card">

                                    <div className="project-stat-icon">
                                        ✓
                                    </div>

                                    <div>

                                        <span>
                                            Completed
                                        </span>

                                        <strong>
                                            {dashboardLoading
                                                ? "..."
                                                : completedTasks}
                                        </strong>

                                    </div>

                                </div>

                                <div className="project-stat-card">

                                    <div className="project-stat-icon">
                                        !
                                    </div>

                                    <div>

                                        <span>
                                            Overdue
                                        </span>

                                        <strong>
                                            {dashboardLoading
                                                ? "..."
                                                : overdueTasks}
                                        </strong>

                                    </div>

                                </div>

                            </section>

                            {/* Dashboard error */}
                            {!dashboardLoading && dashboardError && (
                                <div
                                    className="project-details-error"
                                    style={{ marginTop: "22px" }}
                                >

                                    <p>
                                        {dashboardError}
                                    </p>

                                    <button
                                        onClick={loadDashboard}
                                        className="primary-button"
                                    >
                                        Reload Statistics
                                    </button>

                                </div>
                            )}

                            {/* Tasks */}
                            <section className="project-section-card">

                                <div className="project-section-header">

                                    <div>

                                        <h3>
                                            Project Tasks
                                        </h3>

                                        <p>
                                            Tasks belonging to this project
                                        </p>

                                    </div>

                                    <button
                                        className="primary-button"
                                        onClick={() =>
                                            navigate(
                                                `/projects/${projectId}/tasks`
                                            )
                                        }
                                    >
                                        View Tasks →
                                    </button>

                                </div>

                                <div className="project-empty-state">

                                    <div className="project-empty-icon">
                                        ✓
                                    </div>

                                    <h4>
                                        Tasks will appear here
                                    </h4>

                                    <p>
                                        Open the Tasks page to manage
                                        project tasks.
                                    </p>

                                </div>

                            </section>

                            {/* Team */}
                            <section className="project-section-card">

                                <div className="project-section-header">

                                    <div>

                                        <h3>
                                            Project Team
                                        </h3>

                                        <p>
                                            Members assigned to this project
                                        </p>

                                    </div>

                                    <button
                                        className="secondary-button"
                                        onClick={() =>
                                            navigate(
                                                `/projects/${projectId}/members`
                                            )
                                        }
                                    >
                                        Manage Team →
                                    </button>

                                </div>

                                <div className="project-empty-state">

                                    <div className="project-empty-icon">
                                        ♙
                                    </div>

                                    <h4>
                                        Team members
                                    </h4>

                                    <p>
                                        Project members will appear here.
                                    </p>

                                </div>

                            </section>

                        </>
                    )}

                </div>

            </main>

        </div>
    );
}

export default ProjectDetails;