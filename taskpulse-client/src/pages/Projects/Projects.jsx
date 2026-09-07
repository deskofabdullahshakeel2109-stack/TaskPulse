import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import projectService from "../../services/projectService";
import "./Projects.css";

function Projects() {
    const { user, logout } = useAuth();
    const navigate = useNavigate();

    const [projects, setProjects] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const role = user?.role || "Member";
    const canCreate = ["Admin", "Manager"].includes(role);

    useEffect(() => {
        loadProjects();
    }, []);

    const loadProjects = async () => {
        try {
            setLoading(true);
            setError("");

            const data = await projectService.getAll();

            setProjects(Array.isArray(data) ? data : []);
        } catch (err) {
            setError(
                err.response?.data?.message ||
                "Unable to load projects."
            );
        } finally {
            setLoading(false);
        }
    };

    const handleLogout = () => {
        logout();
        navigate("/login");
    };

    return (
        <div className="projects-page">

            {/* Sidebar */}
            <aside className="projects-sidebar">

                <div className="projects-sidebar-brand">
                    <h1>
                        Task<span>Pulse</span>
                    </h1>
                </div>

                <nav className="projects-sidebar-nav">

                    <button
                        className="projects-nav-item"
                        onClick={() => navigate("/dashboard")}
                    >
                        <span>⌂</span>
                        Dashboard
                    </button>

                    <button
                        className="projects-nav-item active"
                        onClick={() => navigate("/projects")}
                    >
                        <span>▣</span>
                        Projects
                    </button>

                    <button
                        className="projects-nav-item"
                        onClick={() => navigate("/tasks")}
                    >
                        <span>✓</span>
                        Tasks
                    </button>

                    <button
                        className="projects-nav-item"
                        onClick={() => navigate("/notifications")}
                    >
                        <span>●</span>
                        Notifications
                    </button>

                    {(role === "Admin" || role === "Manager") && (
                        <button
                            className="projects-nav-item"
                            onClick={() => navigate("/reports")}
                        >
                            <span>▤</span>
                            Reports
                        </button>
                    )}

                    {role === "Admin" && (
                        <>
                            <button
                                className="projects-nav-item"
                                onClick={() => navigate("/users")}
                            >
                                <span>♙</span>
                                Users
                            </button>

                            <button
                                className="projects-nav-item"
                                onClick={() => navigate("/activity-logs")}
                            >
                                <span>☷</span>
                                Activity Logs
                            </button>
                        </>
                    )}

                </nav>

                <div className="projects-sidebar-bottom">

                    <button
                        className="projects-nav-item"
                        onClick={handleLogout}
                    >
                        <span>↪</span>
                        Logout
                    </button>

                </div>

            </aside>

            {/* Main */}
            <main className="projects-main">

                {/* Header */}
                <header className="projects-header">

                    <div>
                        <h2>Projects</h2>
                        <p>
                            Manage and view your TaskPulse projects
                        </p>
                    </div>

                    <div className="projects-user-info">

                        <div className="projects-user-avatar">
                            {(user?.fullName || user?.email || "U")
                                .charAt(0)
                                .toUpperCase()}
                        </div>

                        <div className="projects-user-details">
                            <strong>
                                {user?.fullName || "User"}
                            </strong>

                            <span>{role}</span>
                        </div>

                    </div>

                </header>

                {/* Content */}
                <div className="projects-content">

                    <div className="projects-title-row">

                        <div>
                            <h3>All Projects</h3>
                            <p>
                                {projects.length} project
                                {projects.length !== 1 ? "s" : ""}
                            </p>
                        </div>

                        {canCreate && (
                            <button
                                className="create-project-button"
                                onClick={() =>
                                    navigate("/projects/create")
                                }
                            >
                                + Create Project
                            </button>
                        )}

                    </div>

                    {loading && (
                        <div className="projects-message">
                            <div className="projects-spinner"></div>
                            <p>Loading projects...</p>
                        </div>
                    )}

                    {!loading && error && (
                        <div className="projects-error">
                            <strong>Unable to load projects</strong>
                            <p>{error}</p>

                            <button onClick={loadProjects}>
                                Try Again
                            </button>
                        </div>
                    )}

                    {!loading && !error && projects.length === 0 && (
                        <div className="projects-empty">

                            <div className="projects-empty-icon">
                                ▣
                            </div>

                            <h3>No Projects Yet</h3>

                            <p>
                                There are currently no projects
                                available for your account.
                            </p>

                            {canCreate && (
                                <button
                                    className="create-project-button"
                                    onClick={() =>
                                        navigate("/projects/create")
                                    }
                                >
                                    + Create Your First Project
                                </button>
                            )}

                        </div>
                    )}

                    {!loading && !error && projects.length > 0 && (
                        <div className="projects-grid">

                            {projects.map((project) => (
                                <div
                                    className="project-card"
                                    key={project.id}
                                >

                                    <div className="project-card-top">

                                        <div className="project-icon">
                                            ▣
                                        </div>

                                        <span className="project-status">
                                            Active
                                        </span>

                                    </div>

                                    <h3>
                                        {project.name}
                                    </h3>

                                    <p className="project-description">
                                        {project.description ||
                                            "No description provided."}
                                    </p>

                                    <div className="project-meta">

                                        <div>
                                            <span>Created</span>
                                            <strong>
                                                {project.createdAt
                                                    ? new Date(
                                                        project.createdAt
                                                    ).toLocaleDateString()
                                                    : "-"}
                                            </strong>
                                        </div>

                                        <div>
                                            <span>Project ID</span>
                                            <strong>
                                                {project.id
                                                    ? project.id
                                                        .substring(0, 8)
                                                    : "-"}
                                            </strong>
                                        </div>

                                    </div>

                                    <button
                                        className="project-view-button"
                                        onClick={() =>
                                            navigate(
                                                `/projects/${project.id}`
                                            )
                                        }
                                    >
                                        Open Project →
                                    </button>

                                </div>
                            ))}

                        </div>
                    )}

                </div>

            </main>

        </div>
    );
}

export default Projects;